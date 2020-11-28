using System.Threading.Tasks;

namespace Interfaces.Repositories
{
    public interface IEddnRepository
    {
        Task ListenToEddn();
    }
}
