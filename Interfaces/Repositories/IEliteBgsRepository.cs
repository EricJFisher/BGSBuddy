using Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Repositories
{
    public interface IEliteBgsRepository
    {
        Task<List<Asset>> GetAssets(string systemName);
        Task<Faction> GetFaction(string factionName);
        Task<SolarSystem> GetSolarSystem(string systemName);
        Task<List<SolarSystem>> GetExpansionTargets(string systemName);
        Task<DateTime> GetTick();
    }
}
