using Entities;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IUserSettingsService
    {
        Task<UserSettings> Load();
        Task Save(UserSettings asset);
    }
}
