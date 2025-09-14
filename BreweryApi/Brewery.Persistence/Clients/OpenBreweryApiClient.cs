using System.Text.Json;
using Brewery.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Brewery.Persistence.Clients
{
    public class OpenBreweryApiClient : IOpenBreweryApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenBreweryApiClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public OpenBreweryApiClient(HttpClient httpClient, ILogger<OpenBreweryApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };
        }

        public async Task<IEnumerable<BreweryEntity>> GetBreweriesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching breweries from OpenBreweryDB API");
                
                var response = await _httpClient.GetAsync("breweries?per_page=200");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var breweries = JsonSerializer.Deserialize<IEnumerable<BreweryEntity>>(jsonContent, _jsonOptions);

                _logger.LogInformation("Successfully fetched {Count} breweries", breweries?.Count() ?? 0);
                return breweries ?? new List<BreweryEntity>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching breweries from OpenBreweryDB API");
                throw;
            }
        }
    }
}