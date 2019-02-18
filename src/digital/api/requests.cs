using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using digital.ProvisionResults;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace digital
{
    public partial class requests
    {
        public static async Task<Guid> CreateDevice(HttpClient httpClient, ILogger logger, DeviceCreate deviceCreate)
        {
            logger.LogInformation($"Creating Device: {JsonConvert.SerializeObject(deviceCreate, Formatting.Indented)}");
            var content = JsonConvert.SerializeObject(deviceCreate);
            var response = await httpClient.PostAsync("devices", new StringContent(content, Encoding.UTF8, "application/json"));
            return await GetIdFromResponse(response, logger);
        }
        public static async Task<DeviceModel> GetDevice(
            HttpClient httpClient,
            ILogger logger,
            Guid id,
            string includes = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("GetDevice requires a non empty guid as id");

            var response = await httpClient.GetAsync($"devices/{id}/" + (includes != null ? $"?includes={includes}" : ""));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var device = JsonConvert.DeserializeObject<DeviceModel>(content);
                logger.LogInformation($"Retrieved Device: {JsonConvert.SerializeObject(device, Formatting.Indented)}");
                return device;
            }

            return null;
        }

        public static async Task<Guid> CreateSpace(HttpClient httpClient, ILogger logger, SpaceCreate spaceCreate)
        {
            logger.LogInformation($"Creating Space: {JsonConvert.SerializeObject(spaceCreate, Formatting.Indented)}");
            var content = JsonConvert.SerializeObject(spaceCreate);
            var response = await httpClient.PostAsync("spaces", new StringContent(content, Encoding.UTF8, "application/json"));
            return await GetIdFromResponse(response, logger);
        }

        public static async Task UpdateUserDefinedFunction(
            HttpClient httpClient,
            ILogger logger,
            UserDefinedFunctionUpdate userDefinedFunction,
            string js)
        {
            logger.LogInformation($"Updating UserDefinedFunction with Metadata: {JsonConvert.SerializeObject(userDefinedFunction, Formatting.Indented)}");
            var displayContent = js.Length > 100 ? js.Substring(0, 100) + "..." : js;
            logger.LogInformation($"Updating UserDefinedFunction with Content: {displayContent}");

            var metadataContent = new StringContent(JsonConvert.SerializeObject(userDefinedFunction), Encoding.UTF8, "application/json");
            metadataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            var multipartContent = new MultipartFormDataContent("userDefinedFunctionBoundary");
            multipartContent.Add(metadataContent, "metadata");
            multipartContent.Add(new StringContent(js), "contents");

            await httpClient.PatchAsync($"userdefinedfunctions/{userDefinedFunction.Id}", multipartContent);
        }

        public static async Task<Space> FindSpace(
            HttpClient httpClient,
            ILogger logger,
            string name,
            Guid parentId)
        {
            var filterName = $"Name eq '{name}'";
            var filterParentSpaceId = parentId != Guid.Empty
                ? $"ParentSpaceId eq guid'{parentId}'"
                : $"ParentSpaceId eq null";
            var odataFilter = $"$filter={filterName} and {filterParentSpaceId}";

            var response = await httpClient.GetAsync($"spaces?{odataFilter}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var spaces = JsonConvert.DeserializeObject<IReadOnlyCollection<Space>>(content);
                var matchingSpace = spaces.SingleOrDefault();
                if (matchingSpace != null)
                {
                    logger.LogInformation($"Retrieved Unique Space using 'name' and 'parentSpaceId': {JsonConvert.SerializeObject(matchingSpace, Formatting.Indented)}");
                    return matchingSpace;
                }
            }
            return null;
        }

        public static async Task<Guid> CreateEndpoints(HttpClient httpClient, ILogger logger, EndpointsCreate endpointCreate)
        {
            logger.LogInformation($"Creating Endpoint: {JsonConvert.SerializeObject(endpointCreate, Formatting.Indented)}");
            var content = JsonConvert.SerializeObject(endpointCreate);
            var response = await httpClient.PostAsync("endpoints", new StringContent(content, Encoding.UTF8, "application/json"));
            return await GetIdFromResponse(response, logger);
        }

         public static async Task<Guid> CreateMatcher(HttpClient httpClient, ILogger logger, MatcherCreate matcherCreate)
        {
            logger.LogInformation($"Creating Matcher: {JsonConvert.SerializeObject(matcherCreate, Formatting.Indented)}");
            var content = JsonConvert.SerializeObject(matcherCreate);
            var response = await httpClient.PostAsync("matchers", new StringContent(content, Encoding.UTF8, "application/json"));
            return await GetIdFromResponse(response, logger);
        }

        public static async Task<Guid> CreateResource(HttpClient httpClient, ILogger logger, ResourceCreate resourceCreate)
        {
            logger.LogInformation($"Creating Resource: {JsonConvert.SerializeObject(resourceCreate, Formatting.Indented)}");
            var content = JsonConvert.SerializeObject(resourceCreate);
            var response = await httpClient.PostAsync("resources", new StringContent(content, Encoding.UTF8, "application/json"));
            return await GetIdFromResponse(response, logger);
        }

        public static async Task<Space> GetSpace(
            HttpClient httpClient,
            ILogger logger,
            Guid id,
            string includes = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("GetSpace requires a non empty guid as id");

            var response = await httpClient.GetAsync($"spaces/{id}/" + (includes != null ? $"?includes={includes}" : ""));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var space = JsonConvert.DeserializeObject<Space>(content);
                logger.LogInformation($"Retrieved Space: {JsonConvert.SerializeObject(space, Formatting.Indented)}");
                return space;
            }

            return null;
        }
public static async Task<UserDefinedFunction> FindUserDefinedFunction(
            HttpClient httpClient,
            ILogger logger,
            string name,
            Guid spaceId)
        {
            var filterNames = $"names={name}";
            var filterSpaceId = $"&spaceIds={spaceId.ToString()}";
            var filter = $"{filterNames}{filterSpaceId}";

            var response = await httpClient.GetAsync($"userdefinedfunctions?{filter}&includes=matchers");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var userDefinedFunctions = JsonConvert.DeserializeObject<IReadOnlyCollection<UserDefinedFunction>>(content);
                var userDefinedFunction = userDefinedFunctions.SingleOrDefault();
                if (userDefinedFunction != null)
                {
                    logger.LogInformation($"Retrieved Unique UserDefinedFunction using 'name' and 'spaceId': {JsonConvert.SerializeObject(userDefinedFunction, Formatting.Indented)}");
                    return userDefinedFunction;
                }
            }
            return null;
        }
        
        public static async Task<DeviceModel> FindDevice(
            HttpClient httpClient,
            ILogger logger,
            string hardwareId,
            Guid? spaceId,
            string includes = null)
        {
            var filterHardwareIds = $"hardwareIds={hardwareId}";
            var filterSpaceId = spaceId != null ? $"&spaceIds={spaceId.ToString()}" : "";
            var includesParam = includes != null ? $"&includes={includes}" : "";
            var filter = $"{filterHardwareIds}{filterSpaceId}{includesParam}";

            var response = await httpClient.GetAsync($"devices?{filter}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var devices = JsonConvert.DeserializeObject<IReadOnlyCollection<DeviceModel>>(content);
                var matchingDevice = devices.SingleOrDefault();
                if (matchingDevice != null)
                {
                    logger.LogInformation($"Retrieved Unique Device using 'hardwareId' and 'spaceId': {JsonConvert.SerializeObject(matchingDevice, Formatting.Indented)}");
                    return matchingDevice;
                }
            }
            return null;
        }
        public static async Task<IEnumerable<Matcher>> FindMatchers(
            HttpClient httpClient,
            ILogger logger,
            IEnumerable<string> names,
            Guid spaceId)
        {
            var commaDelimitedNames = names.Aggregate((string acc, string s) => acc + "," + s);
            var filterNames = $"names={commaDelimitedNames}";
            var filterSpaceId = $"&spaceIds={spaceId.ToString()}";
            var filter = $"{filterNames}{filterSpaceId}";

            var response = await httpClient.GetAsync($"matchers?{filter}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var matchers = JsonConvert.DeserializeObject<IReadOnlyCollection<Matcher>>(content);
                if (matchers != null)
                {
                    logger.LogInformation($"Retrieved Unique Matchers using 'names' and 'spaceId': {JsonConvert.SerializeObject(matchers, Formatting.Indented)}");
                    return matchers;
                }
            }
            return null;
        }

        public static async Task<Guid> CreateSensor(HttpClient httpClient, ILogger logger, SensorCreate sensorCreate)
        {
            logger.LogInformation($"Creating Sensor: {JsonConvert.SerializeObject(sensorCreate, Formatting.Indented)}");
            var content = JsonConvert.SerializeObject(sensorCreate);
            var response = await httpClient.PostAsync("sensors", new StringContent(content, Encoding.UTF8, "application/json"));
            return await GetIdFromResponse(response, logger);
        }

        public static async Task<Guid> CreateUserDefinedFunction(
            HttpClient httpClient,
            ILogger logger,
            UserDefinedFunctionCreate userDefinedFunctionCreate,
            string js)
        {
            logger.LogInformation($"Creating UserDefinedFunction with Metadata: {JsonConvert.SerializeObject(userDefinedFunctionCreate, Formatting.Indented)}");
            var displayContent = js.Length > 100 ? js.Substring(0, 100) + "..." : js;
            logger.LogInformation($"Creating UserDefinedFunction with Content: {displayContent}");

            var metadataContent = new StringContent(JsonConvert.SerializeObject(userDefinedFunctionCreate), Encoding.UTF8, "application/json");
            metadataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            var multipartContent = new MultipartFormDataContent("userDefinedFunctionBoundary");
            multipartContent.Add(metadataContent, "metadata");
            multipartContent.Add(new StringContent(js), "contents");

            var response = await httpClient.PostAsync("userdefinedfunctions", multipartContent);
            return await GetIdFromResponse(response, logger);
        }
        public static async Task<Guid> CreateRoleAssignment(HttpClient httpClient, ILogger logger, RoleAssignmentCreate roleAssignmentCreate)
        {
            logger.LogInformation($"Creating RoleAssignment: {JsonConvert.SerializeObject(roleAssignmentCreate, Formatting.Indented)}");
            var content = JsonConvert.SerializeObject(roleAssignmentCreate);
            var response = await httpClient.PostAsync("roleassignments", new StringContent(content, Encoding.UTF8, "application/json"));
            return await GetIdFromResponse(response, logger);
        }
        public static async Task<Resource> GetResource(
            HttpClient httpClient,
            ILogger logger,
            Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("GetResource requires a non empty guid as id");

            var response = await httpClient.GetAsync($"resources/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var resource = JsonConvert.DeserializeObject<Resource>(content);
                logger.LogInformation($"Retrieved Resource: {JsonConvert.SerializeObject(resource, Formatting.Indented)}");
                return resource;
            }

            return null;
        }

        private static async Task<Guid> GetIdFromResponse(HttpResponseMessage response, ILogger logger)
        {
            if (!response.IsSuccessStatusCode)
                return Guid.Empty;

            var content = await response.Content.ReadAsStringAsync();

            // strip out the double quotes that come in the response and parse into a guid
            if (!Guid.TryParse(content.Substring(1, content.Length - 2), out var createdId))
            {
                logger.LogError($"Returned value from POST did not parse into a guid: {content}");
                return Guid.Empty;
            }

            return createdId;
        }

        public static async Task<IEnumerable<Sensor>> GetSensorsOfSpace(
            HttpClient httpClient,
            ILogger logger,
            Guid spaceId)
        {
            var response = await httpClient.GetAsync($"sensors?spaceId={spaceId.ToString()}&includes=Types");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var sensors = JsonConvert.DeserializeObject<IEnumerable<Sensor>>(content);
                logger.LogInformation($"Retrieved {sensors.Count()} Sensors");
                return sensors;
            }
            else
            {
                return Array.Empty<Sensor>();
            }
        }

    }
}