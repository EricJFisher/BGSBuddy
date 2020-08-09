using Entities;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface ISolarSystemsService
    {
        Task<SolarSystem> Get(string systemName);
    }
}
