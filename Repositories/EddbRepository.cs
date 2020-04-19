using Entities;
using Repositories.EddbRequestTypes;
using System.Net.Http;
using System.Threading.Tasks;

namespace Repositories
{
    public class EddbRepository
    {
        private readonly string baseUrl = "https://eddbapi.kodeblox.com/api/v4/";
        private HttpClient client = new HttpClient();
        private FileSystemRepository _fileSystemRepository;

        public EddbRepository()
        {
            _fileSystemRepository = new FileSystemRepository();
        }

        public async Task<SolarSystem> GetSystem(string name)
        {
            var file = "SI" + name + ".log";
            var json = await _fileSystemRepository.RetrieveJsonFromFile(file);
            if (string.IsNullOrEmpty(json))
                json = await FetchSystem(name).ConfigureAwait(false);
            return await ConvertJsonToSystem(json).ConfigureAwait(false);
        }

        public async Task<string> FetchSystem(string name)
        {
            var file = "SI" + name + ".log";
            var url = baseUrl + "populatedsystems?name=" + name;
            var response = await client.GetStringAsync(url).ConfigureAwait(false);
            await _fileSystemRepository.SaveJsonToFile(response, file);
            return response;
        }

        private async Task<SolarSystem> ConvertJsonToSystem(string json)
        {
            var system = new SolarSystem();

            var request = EddbSystemRequest.FromJson(json);
            system.Name = request.Docs[0].Name;
            system.ControllingFaction = request.Docs[0].ControllingMinorFaction;
            foreach(var faction in request.Docs[0].MinorFactionPresences)
            {
                var subFaction = new SubFaction();
                subFaction.Name = faction.Id;
                subFaction.Influence = faction.Influence;
                system.SubFactions.Add(subFaction);
            }

            return system;
        }
    }
}
