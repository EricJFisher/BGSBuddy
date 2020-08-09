using Interfaces.Repositories;
using Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Services
{
    public class TickService : ITickService
    {
        private readonly IEliteBgsRepository _eliteBgsRepository;

        public TickService(IEliteBgsRepository eliteBgsRepository)
        {
            _eliteBgsRepository = eliteBgsRepository;
        }

        public async Task<DateTime> Get()
        {
            return await _eliteBgsRepository.GetTick();
        }
    }
}
