using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface ISolarSystemsService
    {
        Task<SolarSystem> Get(string systemName);
        Task<ExpansionReport> GetExpansionTargets(ExpansionReport expansionReport);
    }
}
