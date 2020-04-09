using Entities;
using Repositories.EliteBgsTypes;
using System.Net.Http;
using System.Threading.Tasks;

namespace Repositories
{
    public class EliteBgsRepository
    {
        private readonly string baseUrl = "https://elitebgs.app/api/ebgs/v4/";
        private HttpClient client = new HttpClient();
        private FileSystemRepository _fileSystemRepository;

        public EliteBgsRepository(FileSystemRepository fileSystemRepository)
        {
            _fileSystemRepository = fileSystemRepository;
        }

        public async Task FetchFaction(string name)
        {
            var file = "F" + name + ".log";
            var url = baseUrl + "factions?name=" + name;
            var response = await client.GetStringAsync(url);
            await _fileSystemRepository.SaveJsonToFile(response, file);
        }

        private async Task<Faction> ConvertJsonToFaction(string json)
        {
            var faction = new Faction();

            var request = EliteBgsFactionRequest.FromJson(json);
            faction.Name = request.Docs[0].Name;
            faction.UpdatedOn = request.Docs[0].UpdatedAt.UtcDateTime;
            
            foreach(var system in request.Docs[0].FactionPresence)
            {
                var solarSystem = new SolarSystem();
                solarSystem.Name = system.SystemName;
                if (system.Conflicts.Count > 0)
                {
                    solarSystem.ConflictType = system.Conflicts[0].Type;
                    solarSystem.ConflictStatus = system.Conflicts[0].Status;
                }
            }

            return faction;
        }

        public async Task FetchSystem(string name)
        {
            var file = "S" + name + ".log";
            var url = baseUrl + "systems?name=" + name;
            var response = await client.GetStringAsync(url);
            await _fileSystemRepository.SaveJsonToFile(response, file);
        }

        public async Task FetchStation(string name)
        {
            var file = "A" + name + ".log";
            var url = baseUrl + "stations?name=" + name;
            var response = await client.GetStringAsync(url);
            await _fileSystemRepository.SaveJsonToFile(response, file);
        }
    }
}
