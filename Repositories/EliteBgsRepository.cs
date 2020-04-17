using Entities;
using Repositories.EliteBgsTypes.FactionRequest;
using Repositories.EliteBgsTypes.SystemRequest;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Faction = Entities.Faction;

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

        public async Task<Faction> GetFaction(string name)
        {
            var file = "F" + name + ".log";
            var json = await _fileSystemRepository.RetrieveJsonFromFile(file).ConfigureAwait(false);
            if(string.IsNullOrEmpty(json))
            {
                json = await FetchFaction(name).ConfigureAwait(false);
            }
            return await ConvertJsonToFaction(json).ConfigureAwait(false);
        }

        public async Task<SolarSystem> GetSystem(string name)
        {
            var file = "S" + name + ".log";
            var json = await _fileSystemRepository.RetrieveJsonFromFile(file);
            if (string.IsNullOrEmpty(json))
                json = await FetchSystem(name);
            return await ConvertJsonToSystem(json);
        }

        public async Task<string> FetchFaction(string name)
        {
            var file = "F" + name + ".log";
            var url = baseUrl + "factions?name=" + name;
            var response = await client.GetStringAsync(url).ConfigureAwait(false);
            await _fileSystemRepository.SaveJsonToFile(response, file);
            return response;
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

                solarSystem.ControllingFaction = (await GetSystem(system.SystemName).ConfigureAwait(false)).ControllingFaction;

                faction.SolarSystems.Add(solarSystem);
            }

            return faction;
        }

        private async Task<SolarSystem> ConvertJsonToSystem(string json)
        {
            var system = new SolarSystem();

            var request = EliteBgsSystemRequest.FromJson(json);
            system.Name = request.Docs[0].Name;
            system.ControllingFaction = request.Docs[0].ControllingMinorFaction;

            return system;
        }

        public async Task<string> FetchSystem(string name)
        {
            var file = "S" + name + ".log";
            var url = baseUrl + "systems?name=" + name;
            var response = await client.GetStringAsync(url).ConfigureAwait(false);
            await _fileSystemRepository.SaveJsonToFile(response, file);
            return response;
        }

        public async Task<string> FetchStation(string systemName)
        {
            var file = "A" + name + ".log";
            var url = baseUrl + "stations?system=" + systemName;
            var response = await client.GetStringAsync(url).ConfigureAwait(false);
            await _fileSystemRepository.SaveJsonToFile(response, file);
            return response;
        }
    }
}
