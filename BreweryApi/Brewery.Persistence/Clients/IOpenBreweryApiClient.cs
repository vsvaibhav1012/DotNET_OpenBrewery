using Brewery.Core.Entities;

namespace Brewery.Persistence.Clients
{
    public interface IOpenBreweryApiClient
    {
        Task<IEnumerable<BreweryEntity>> GetBreweriesAsync();
    }
}