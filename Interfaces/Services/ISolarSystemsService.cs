using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface ISolarSystemsService
    {
        Task<List<SolarSystem>> GetByFactionName(string factionName);
        Task<ExpansionReport> GetExpansionTargets(ExpansionReport expansionReport);
    }
}
