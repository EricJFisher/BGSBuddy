using Entities;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IFactionsService
    {
        Task<Faction> Get(string factionName);
    }
}
