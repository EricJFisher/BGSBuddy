using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Repositories
{
    public interface IEddbRepository
    {
        public Task<List<string>> GetHomeFactions(string systemName);
    }
}
