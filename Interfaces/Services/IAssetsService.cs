using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IAssetsService
    {
        Task<List<Asset>> GetAssets(string systemName);
    }
}
