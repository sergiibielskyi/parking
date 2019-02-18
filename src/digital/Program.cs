using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace digital
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var appSettings = AppSettings.Load();

            var action = ParseArgs(args);
                if (action == null)
                    return;

                switch (action)
                {
                    case Action.CreateEndpoints:
                        await Actions.CreateEndpoints(await SetupHttpClient(Loggers.ConsoleLogger, appSettings), Loggers.ConsoleLogger);
                        break;
                    case Action.ProvisionParking:
                        await Actions.ProvisionParking(await SetupHttpClient(Loggers.ConsoleLogger, appSettings), Loggers.ConsoleLogger);
                        break;

                    default:
                        throw new NotImplementedException();
                }
        }

        private static async Task<HttpClient> SetupHttpClient(ILogger logger, AppSettings appSettings)
        {
            var httpClient = new HttpClient(new LoggingHttpHandler(logger))
            {
                BaseAddress = new Uri(appSettings.BaseUrl),
            };
            var accessToken = (await Authentication.GetToken(logger, appSettings));

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            return httpClient;
        }

        private static Action? ParseArgs(string[] args)
        {
            if (args.Length >= 1 && Enum.TryParse(args[0], out Action actionName))
            {
                return actionName;
            }
            else
            {
                // Generate the list of available action names from the enum
                // and output them in the usage string
                var actionNames = Enum.GetNames(typeof(Action))
                    .Aggregate((string acc, string s) => acc + " | " + s);
                Console.WriteLine($"Usage: dotnet run [{actionNames}]");

                return null;
            }
        }
    }
}
