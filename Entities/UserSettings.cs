using Interfaces.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Entities
{
    public class UserSettings
    {
        private readonly IFileSystemRepository _fileSystemRepository;

        public string FactionName { get; set; } = string.Empty;
        public List<string> OffLimits { get; set; } = new List<string>();

        private UserSettings()
        {

        }

        public UserSettings(IFileSystemRepository fileSystemRepository)
        {
            _fileSystemRepository = fileSystemRepository;
        }

        public async Task Load()
        {
            UserSettings settings = new UserSettings();
            var json = _fileSystemRepository.RetrieveJsonFromFile("Settings.txt");
            if (!string.IsNullOrEmpty(json))
                settings = JsonConvert.DeserializeObject<UserSettings>(json);
            FactionName = settings.FactionName;
            OffLimits = settings.OffLimits;
        }

        public async Task Save()
        {
            var json = JsonConvert.SerializeObject(this);
            _fileSystemRepository.SaveJsonToFile(json, "Settings.txt");
        }
    }
}
