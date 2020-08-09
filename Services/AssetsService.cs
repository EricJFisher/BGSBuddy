using Entities;
using Interfaces.Repositories;
using Interfaces.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class AssetsService : IAssetsService
    {
        private IEliteBgsRepository _eliteBgsRepository;

        public AssetsService(IEliteBgsRepository eliteBgsRepository)
        {
            _eliteBgsRepository = eliteBgsRepository;
        }

        public async Task<List<Asset>> GetAssets(string systemName)
        {
            return await _eliteBgsRepository.GetAssets(systemName);
        }
    }
}
