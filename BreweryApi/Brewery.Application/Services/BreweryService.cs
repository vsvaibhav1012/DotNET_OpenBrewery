using Brewery.Application.DTOs;
using Brewery.Application.Interfaces;
using Brewery.Core.Entities;
using Brewery.Core.Enums;
using Brewery.Persistence.Repositories;
using Microsoft.Extensions.Logging;

namespace Brewery.Application.Services
{
    public class BreweryService : IBreweryService
    {
        private readonly IBreweryRepository _repository;
        private readonly ILogger<BreweryService> _logger;

        public BreweryService(IBreweryRepository repository, ILogger<BreweryService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<PagedResult<BreweryDto>>> GetBreweriesAsync(BrewerySearchRequest request)
        {
            try
            {
                var breweries = await _repository.GetAllAsync();
                var filteredBreweries = FilterBreweries(breweries, request);
                var sortedBreweries = SortBreweries(filteredBreweries, request);
                
                var totalCount = sortedBreweries.Count();
                var pagedBreweries = sortedBreweries
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(b => MapToDto(b, request.UserLatitude, request.UserLongitude));

                var result = new PagedResult<BreweryDto>
                {
                    Items = pagedBreweries,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };

                return new ApiResponse<PagedResult<BreweryDto>>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving breweries");
                return new ApiResponse<PagedResult<BreweryDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving breweries"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetAutocompleteAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
                {
                    return new ApiResponse<IEnumerable<string>>
                    {
                        Success = true,
                        Data = new List<string>()
                    };
                }

                var breweries = await _repository.GetAllAsync();
                var suggestions = breweries
                    .Where(b => b.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .Select(b => b.Name)
                    .Distinct()
                    .Take(10)
                    .OrderBy(name => name);

                return new ApiResponse<IEnumerable<string>>
                {
                    Success = true,
                    Data = suggestions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting autocomplete suggestions");
                return new ApiResponse<IEnumerable<string>>
                {
                    Success = false,
                    Message = "An error occurred while getting suggestions"
                };
            }
        }

        public async Task<ApiResponse<BreweryDto?>> GetBreweryByIdAsync(string id)
        {
            try
            {
                var brewery = await _repository.GetByIdAsync(id);
                if (brewery == null)
                {
                    return new ApiResponse<BreweryDto?>
                    {
                        Success = false,
                        Message = "Brewery not found"
                    };
                }

                return new ApiResponse<BreweryDto?>
                {
                    Success = true,
                    Data = MapToDto(brewery)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving brewery with ID: {Id}", id);
                return new ApiResponse<BreweryDto?>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the brewery"
                };
            }
        }

        private IEnumerable<BreweryEntity> FilterBreweries(IEnumerable<BreweryEntity> breweries, BrewerySearchRequest request)
        {
            var filtered = breweries.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                filtered = filtered.Where(b => 
                    b.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.City))
            {
                filtered = filtered.Where(b => 
                    b.City.Contains(request.City, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.State))
            {
                filtered = filtered.Where(b => 
                    b.State.Contains(request.State, StringComparison.OrdinalIgnoreCase));
            }

            return filtered;
        }

        private IEnumerable<BreweryEntity> SortBreweries(IEnumerable<BreweryEntity> breweries, BrewerySearchRequest request)
        {
            return request.SortBy switch
            {
                SortBy.Name => request.SortOrder == SortOrder.Asc 
                    ? breweries.OrderBy(b => b.Name) 
                    : breweries.OrderByDescending(b => b.Name),
                SortBy.City => request.SortOrder == SortOrder.Asc 
                    ? breweries.OrderBy(b => b.City) 
                    : breweries.OrderByDescending(b => b.City),
                SortBy.Distance => SortByDistance(breweries, request),
                _ => breweries.OrderBy(b => b.Name)
            };
        }

        private IEnumerable<BreweryEntity> SortByDistance(IEnumerable<BreweryEntity> breweries, BrewerySearchRequest request)
        {
            if (!request.UserLatitude.HasValue || !request.UserLongitude.HasValue)
            {
                return breweries.OrderBy(b => b.Name);
            }

            var breweriesWithDistance = breweries
                .Where(b => b.Latitude.HasValue && b.Longitude.HasValue)
                .Select(b => new
                {
                    Brewery = b,
                    Distance = CalculateDistance(
                        request.UserLatitude.Value, request.UserLongitude.Value,
                        b.Latitude!.Value, b.Longitude!.Value)
                });

            return request.SortOrder == SortOrder.Asc
                ? breweriesWithDistance.OrderBy(x => x.Distance).Select(x => x.Brewery)
                : breweriesWithDistance.OrderByDescending(x => x.Distance).Select(x => x.Brewery);
        }

        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth's radius in kilometers
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180;

        private BreweryDto MapToDto(BreweryEntity brewery, double? userLat = null, double? userLon = null)
        {
            var dto = new BreweryDto
            {
                Id = brewery.Id,
                Name = brewery.Name,
                City = brewery.City,
                State = brewery.State,
                Phone = brewery.Phone,
                WebsiteUrl = brewery.WebsiteUrl
            };

            if (userLat.HasValue && userLon.HasValue && brewery.Latitude.HasValue && brewery.Longitude.HasValue)
            {
                dto.Distance = CalculateDistance(userLat.Value, userLon.Value, brewery.Latitude.Value, brewery.Longitude.Value);
            }

            return dto;
        }
    }
}