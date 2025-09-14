using Brewery.Application.DTOs;

namespace Brewery.Application.Interfaces
{
    public interface IBreweryService
    {
        Task<ApiResponse<PagedResult<BreweryDto>>> GetBreweriesAsync(BrewerySearchRequest request);
        Task<ApiResponse<IEnumerable<string>>> GetAutocompleteAsync(string searchTerm);
        Task<ApiResponse<BreweryDto?>> GetBreweryByIdAsync(string id);
    }
}