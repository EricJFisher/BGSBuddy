using Entities;
using Interfaces.Repositories;
using Interfaces.Services;
using System.Threading.Tasks;

namespace Services
{
    public class FactionsService : IFactionsService
    {
        private readonly IEliteBgsRepository _eliteBgsRepository;

        public FactionsService(IEliteBgsRepository eliteBgsRepository)
        {
            _eliteBgsRepository = eliteBgsRepository;
        }

        public async Task<Faction> Get(string factionName)
        {
            return await _eliteBgsRepository.GetFaction(factionName);
        }
    }
}
