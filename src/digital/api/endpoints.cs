using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace digital
{
    public class EndpointsCreate
    {
        public string ConnectionString { get; set; }
        public string[] EventTypes { get; set; }
        public string Path { get; set; }
        public string SecondaryConnectionString { get; set; }
        public string Type { get; set; }
   }
    

    public static partial class Actions
    {
        public static EndpointsCreate ToEndpointCreate(this EndpointDescription description)
            => new EndpointsCreate()
            {
                ConnectionString = description.connectionString,
                EventTypes = description.eventTypes,
                Path = description.path,
                SecondaryConnectionString = description.secondaryConnectionString,
                Type = description.type,
            };
        private static string GetCreationSummary(string itemTypeSingular, string itemTypePlural, List<Guid> createdIds)
            => createdIds.Count == 0
                ? $"Created 0 {itemTypePlural}."
                : createdIds.Count == 1
                    ? $"Created 1 {itemTypeSingular}: {AggregateIdsIntoString(createdIds)}"
                    : $"Created {createdIds.Count} {itemTypePlural}: {AggregateIdsIntoString(createdIds)}";
        private static string AggregateIdsIntoString(IEnumerable<Guid> ids) 
            => ids
                .Select(id => id.ToString())
                .Aggregate((acc, cur) => acc + ", " + cur);

        public static async Task<IEnumerable<Guid>> CreateEndpoints(HttpClient httpClient, ILogger logger)
        {
            IEnumerable<EndpointDescription> endpointDescriptions;
            using (var r = new StreamReader("config/Endpoints.yaml"))
            {
                endpointDescriptions = await GetCreateEndpointsDescriptions(r);
            }

            var createdIds = (await CreateEndpoints(httpClient, logger, endpointDescriptions)).ToList();

            Console.WriteLine($"CreateEndpoints completed. {GetCreationSummary("endpoint", "endpoints", createdIds)}");

            return createdIds;
        }

        public static async Task<IEnumerable<EndpointDescription>> GetCreateEndpointsDescriptions(TextReader textReader)
            => new Deserializer().Deserialize<IEnumerable<EndpointDescription>>(await textReader.ReadToEndAsync());

        public static async Task<IEnumerable<Guid>> CreateEndpoints(
            HttpClient httpClient,
            ILogger logger,
            IEnumerable<EndpointDescription> descriptions)
        {
            var endpointIds = new List<Guid>();
            foreach (var description in descriptions)
            {
                endpointIds.Add(await requests.CreateEndpoints(httpClient, logger, description.ToEndpointCreate()));
            }
            return endpointIds.Where(id => id != Guid.Empty);
        }
    }
}