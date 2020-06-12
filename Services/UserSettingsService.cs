using Entities;
using Interfaces.Repositories;
using Interfaces.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IFileSystemRepository _fileSystemRepository;

        public UserSettingsService(IFileSystemRepository fileSystemRepository)
        {
            _fileSystemRepository = fileSystemRepository;
        }

        public async Task<UserSettings> Load()
        {
            UserSettings settings = new UserSettings();
            var json = _fileSystemRepository.RetrieveJsonFromFile("Settings.txt");
            if (!string.IsNullOrEmpty(json))
                settings = JsonConvert.DeserializeObject<UserSettings>(json);
            return settings;
        }

        public async Task Save(UserSettings userSettings)
        {
            var json = JsonConvert.SerializeObject(userSettings);
            _fileSystemRepository.SaveJsonToFile(json, "Settings.txt");
        }
    }
}
