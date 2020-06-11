using Entities;
using Repositories.EddbRequestTypes;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

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

        public async Task<SolarSystem> GetSystem(string name, DateTime lastTick, bool forceUpdate = false)
        {
            var file = "SI" + name + ".log";
            var json = _fileSystemRepository.RetrieveJsonFromFile(file);
            if (string.IsNullOrEmpty(json) || forceUpdate)
                json = await FetchSystem(name).ConfigureAwait(false);
            var system = ConvertJsonToSystem(json);
            if (system.UpdatedOn < DateTime.UtcNow && !forceUpdate)
                system = await GetSystem(name, lastTick, true);
            return system;
        }

        public async Task<string> FetchSystem(string name)
        {
            var file = "SI" + name + ".log";
            var url = baseUrl + "populatedsystems?name=" + HttpUtility.UrlEncode(name);
            var response = await client.GetStringAsync(url).ConfigureAwait(false);
            _fileSystemRepository.SaveJsonToFile(response, file);
            return response;
        }

        private SolarSystem ConvertJsonToSystem(string json)
        {
            var system = new SolarSystem();

            var request = EddbSystemRequest.FromJson(json);
            system.Name = request.Docs[0].Name;
            system.ControllingFaction = request.Docs[0].ControllingMinorFaction;
            system.UpdatedOn = request.Docs[0].UpdatedAt.UtcDateTime;
            foreach (var state in request.Docs[0].States)
            {
                system.States.Add(state.Name);
            }
            foreach (var faction in request.Docs[0].MinorFactionPresences)
            {
                var subFaction = new SubFaction();
                subFaction.Name = faction.Id;
                subFaction.Influence = faction.Influence;
                foreach (var state in faction.ActiveStates)
                {
                    subFaction.ActiveStates.Add(state.Name);
                }
                foreach (var state in faction.PendingStates)
                {
                    subFaction.PendingStates.Add(state.Name);
                }
                system.SubFactions.Add(subFaction);
            }

            return system;
        }
    }
}
