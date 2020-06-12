using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Repositories
{
    public interface IEliteBgsRepository
    {
        Task<List<Asset>> GetAssets(string systemName);
        Task<SolarSystem> GetSolarSystem(string systemName);
    }
}
