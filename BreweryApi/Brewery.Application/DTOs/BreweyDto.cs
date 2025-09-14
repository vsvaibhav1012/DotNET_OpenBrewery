namespace Brewery.Application.DTOs
{
    public class BreweryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string WebsiteUrl { get; set; } = string.Empty;
        public double? Distance { get; set; }
    }

    public class BrewerySearchRequest
    {
        public string? SearchTerm { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public Core.Enums.SortBy SortBy { get; set; } = Core.Enums.SortBy.Name;
        public Core.Enums.SortOrder SortOrder { get; set; } = Core.Enums.SortOrder.Asc;
        public double? UserLatitude { get; set; }
        public double? UserLongitude { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}