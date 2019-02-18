using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;

namespace simulate
{
    class Program
    {
        static IConfigurationSection settings;
        static void Main(string[] args)
        {
            settings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("Settings");

            
            try
            {
                DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(settings["DeviceConnectionString"]);

                if (deviceClient == null)
                {
                    Console.WriteLine("ERROR: Failed to create DeviceClient!");
                    return;
                }

                SendMessage(deviceClient).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXIT: Unexpected error: {0}", ex.Message);
            }

        }

        static async Task SendMessage(DeviceClient deviceClient)
        {
            var serializer = new DataContractJsonSerializer(typeof(Telemetry));

            var sensors = settings.GetSection("Sensors").Get<Sensor[]>();

            var delayPerMessageSend = int.Parse(settings["MessageIntervalInSeconds"]);
            var countOfSendsPerIteration = sensors.Length;
            var maxSecondsToRun = 15 * 60;
            var maxIterations = maxSecondsToRun / countOfSendsPerIteration / delayPerMessageSend;
            var curIteration = 0;

            do {
                foreach (var sensor in sensors)
                {
                    var motionValue = "false";
                    if  (sensor.DataType == "Motion")
                    {
                        if (curIteration % 6 < 3)
                            motionValue = "false";
                        else
                            motionValue = "true";
                    }

                    var telemetryMessage = new Telemetry()
                    {
                        SensorValue = motionValue,
                    };

                    using (var stream = new MemoryStream())
                    {
                        serializer.WriteObject(stream, telemetryMessage);
                        var byteArray = stream.ToArray();
                        Message eventMessage = new Message(byteArray);
                        eventMessage.Properties.Add("DigitalTwins-Telemetry", "1.0");
                        eventMessage.Properties.Add("DigitalTwins-SensorHardwareId", $"{sensor.HardwareId}");
                        eventMessage.Properties.Add("CreationTimeUtc", DateTime.UtcNow.ToString("o"));
                        eventMessage.Properties.Add("x-ms-client-request-id", Guid.NewGuid().ToString());

                        Console.WriteLine($"\t{DateTime.UtcNow.ToLocalTime()}> Sending message: {Encoding.UTF8.GetString(eventMessage.GetBytes())} Properties: {{ {eventMessage.Properties.Aggregate(new StringBuilder(), (sb, x) => sb.Append($"'{x.Key}': '{x.Value}',"), sb => sb.ToString())} }}");

                        await deviceClient.SendEventAsync(eventMessage);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(delayPerMessageSend));

            } while (++curIteration < maxIterations);

            Console.WriteLine($"Finished sending {curIteration} events (per sensor type)");
        }
    }

    public class Sensor
    {
        public string DataType { get; set; }
        public string HardwareId { get; set; }
    }
}
