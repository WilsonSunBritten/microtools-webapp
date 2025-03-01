using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MicrotoolsWebApp.Core;
using MicrotoolsWebApp.Core.Models;

namespace MicrotoolsWebApp.Functions
{
    public class MicrotoolsApi
    {
        private readonly ILogger _logger;
        private readonly MicrotoolRegistry _registry;
        private readonly MicrotoolLoader _loader;
        private readonly JsonSerializerOptions _jsonOptions;

        public MicrotoolsApi(ILoggerFactory loggerFactory, MicrotoolRegistry registry, MicrotoolLoader loader)
        {
            _logger = loggerFactory.CreateLogger<MicrotoolsApi>();
            _registry = registry;
            _loader = loader;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        [Function("ListMicrotools")]
        public HttpResponseData ListMicrotools([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "microtools")] HttpRequestData req)
        {
            _logger.LogInformation("Listing all microtools");

            var microtools = _registry.GetAllMicrotools().Select(m => new
            {
                id = m.Metadata.Id,
                name = m.Metadata.Name,
                description = m.Metadata.Description,
                version = m.Metadata.Version,
                author = m.Metadata.Author,
                tags = m.Metadata.Tags
            });

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(JsonSerializer.Serialize(microtools, _jsonOptions));

            return response;
        }

        [Function("GetMicrotool")]
        public HttpResponseData GetMicrotool([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "microtools/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation($"Getting microtool with ID: {id}");

            var microtool = _registry.GetMicrotoolById(id);
            if (microtool == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                notFoundResponse.WriteString($"Microtool with ID '{id}' not found");
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            
            var result = new
            {
                id = microtool.Metadata.Id,
                name = microtool.Metadata.Name,
                description = microtool.Metadata.Description,
                version = microtool.Metadata.Version,
                author = microtool.Metadata.Author,
                tags = microtool.Metadata.Tags,
                uiSchema = microtool.UiSchema
            };
            
            response.WriteString(JsonSerializer.Serialize(result, _jsonOptions));

            return response;
        }

        [Function("ExecuteMicrotool")]
        public async Task<HttpResponseData> ExecuteMicrotool(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "microtools/{id}/execute")] HttpRequestData req,
            string id)
        {
            _logger.LogInformation($"Executing microtool with ID: {id}");

            var microtool = _registry.GetMicrotoolById(id);
            if (microtool == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                notFoundResponse.WriteString($"Microtool with ID '{id}' not found");
                return notFoundResponse;
            }

            try
            {
                // Load the microtool instance if not already loaded
                if (microtool.Instance == null)
                {
                    microtool.Instance = _loader.LoadMicrotoolFromDirectory(microtool.DirectoryPath, id);
                    
                    if (microtool.Instance == null)
                    {
                        var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                        errorResponse.WriteString($"Failed to load microtool with ID '{id}'");
                        return errorResponse;
                    }
                }

                // Parse the request body to get the parameters
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var parameters = JsonSerializer.Deserialize<Dictionary<string, object>>(requestBody, _jsonOptions);

                // Execute the microtool
                var result = await microtool.Instance.ExecuteAsync(parameters ?? new Dictionary<string, object>());

                // Return the result
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                response.WriteString(JsonSerializer.Serialize(result, _jsonOptions));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing microtool with ID '{id}'");
                
                // Log the full exception details
                Console.WriteLine($"Exception type: {ex.GetType().FullName}");
                Console.WriteLine($"Exception message: {ex.Message}");
                Console.WriteLine($"Exception stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception type: {ex.InnerException.GetType().FullName}");
                    Console.WriteLine($"Inner exception message: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner exception stack trace: {ex.InnerException.StackTrace}");
                }
                
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                errorResponse.WriteString($"Error executing microtool: {ex.Message}");
                return errorResponse;
            }
        }
    }
}
