using System;
using System.Runtime.Serialization;

namespace simulate
{
    [DataContract(Name="Telemetry")]
    public class Telemetry
    {
        [DataMember(Name="SensorValue")]
        public string SensorValue { get; set; }
    }
}