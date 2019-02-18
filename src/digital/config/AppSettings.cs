using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace digital
{
    public class AppSettings {
        // Note: this is a constant because it is the same for every user authorizing
        // against the Digital Twins Apis
        private static string DigitalTwinsAppId = "0b07f429-9f4b-4714-9392-cc5e8e80c8b0";

        public string AADInstance { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Resource { get; set; } = DigitalTwinsAppId;
        public string Tenant { get; set; }
        public string BaseUrl { get; set; }
        public string Authority => AADInstance + Tenant;

        public static AppSettings Load()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config/appSettings.json")
                .Build()
                .Get<AppSettings>();

            // Sanitize input

            // This is because httpClient will behave differently if the
            // passed in Uri has a trailing slash when using GetAsync.
            appSettings.BaseUrl = EnsureTrailingSlash(appSettings.BaseUrl);

            return appSettings;
        }

        private static string EnsureTrailingSlash(string baseUrl)
            => baseUrl.Length == 0 || baseUrl[baseUrl.Length-1] == '/'
                ? baseUrl
                : baseUrl + '/';
    }
}