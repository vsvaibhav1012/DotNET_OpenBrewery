using Brewery.Core.Entities;

namespace Brewery.Persistence.Repositories
{
    public interface IBreweryRepository
    {
        Task<IEnumerable<BreweryEntity>> GetAllAsync();
        Task<BreweryEntity?> GetByIdAsync(string id);
        Task RefreshCacheAsync();
    }
}