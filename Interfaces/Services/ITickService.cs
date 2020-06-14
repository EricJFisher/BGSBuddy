using System;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface ITickService
    {
        Task<DateTime> Get();
    }
}
