using Microsoft.Extensions.Logging;

namespace digital
{
    public static class Loggers
    {
        public static ILogger SilentLogger =
            new Microsoft.Extensions.Logging.LoggerFactory()
                .CreateLogger("DigitalTwinsParking");
        public static ILogger ConsoleLogger =
            new Microsoft.Extensions.Logging.LoggerFactory()
                .AddConsole(LogLevel.Trace)
                .CreateLogger("DigitalTwinsParking");
    }
}