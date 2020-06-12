using Entities;
using Interfaces.Repositories;
using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
