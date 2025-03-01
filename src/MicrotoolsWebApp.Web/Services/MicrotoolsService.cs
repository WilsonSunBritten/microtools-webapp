using System.Net.Http.Json;
using System.Text.Json;
using MicrotoolsWebApp.Core.Models;

namespace MicrotoolsWebApp.Web.Services
{
    /// <summary>
    /// Service for interacting with the microtools API
    /// </summary>
    public class MicrotoolsService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrotoolsService"/> class
        /// </summary>
        /// <param name="httpClient">The HTTP client</param>
        public MicrotoolsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// Gets all microtools
        /// </summary>
        /// <returns>A collection of microtool metadata</returns>
        public async Task<IEnumerable<MicrotoolMetadata>> GetMicrotoolsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<MicrotoolMetadata>>("api/microtools", _jsonOptions);
        }

        /// <summary>
        /// Gets a microtool by its ID
        /// </summary>
        /// <param name="id">The ID of the microtool</param>
        /// <returns>The microtool details</returns>
        public async Task<MicrotoolDetails> GetMicrotoolAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<MicrotoolDetails>($"api/microtools/{id}", _jsonOptions);
        }

        /// <summary>
        /// Executes a microtool
        /// </summary>
        /// <param name="id">The ID of the microtool</param>
        /// <param name="parameters">The parameters for the microtool</param>
        /// <returns>The result of the execution</returns>
        public async Task<object> ExecuteMicrotoolAsync(string id, Dictionary<string, object> parameters)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/microtools/{id}/execute", parameters);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(content, _jsonOptions);
        }
    }

    /// <summary>
    /// Contains detailed information about a microtool
    /// </summary>
    public class MicrotoolDetails
    {
        /// <summary>
        /// Gets or sets the ID of the microtool
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the microtool
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the microtool
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the version of the microtool
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the author of the microtool
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the tags associated with the microtool
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the UI schema for the microtool
        /// </summary>
        public UiSchema UiSchema { get; set; }
    }
}
