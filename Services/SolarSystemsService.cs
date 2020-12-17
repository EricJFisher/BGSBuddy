using Entities;
using Interfaces.Repositories;
using Interfaces.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class SolarSystemsService : ISolarSystemsService
    {
        private IEliteBgsRepository _eliteBgsRepository;

        public SolarSystemsService(IEliteBgsRepository eliteBgsRepository)
        {
            _eliteBgsRepository = eliteBgsRepository;
        }

        public async Task<SolarSystem> Get(string systemName)
        {
            return await _eliteBgsRepository.GetSolarSystem(systemName);
        }

        public async Task<List<SolarSystem>> GetExpansionTargets(string systemName)
        {
            return await _eliteBgsRepository.GetExpansionTargets(systemName);
        }
    }
}
