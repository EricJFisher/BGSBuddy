using Entities;
using Repositories.EddbRequestTypes;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Repositories
{
    public class EddbRepository
    {
        private readonly string baseUrl = "https://eddbapi.kodeblox.com/api/v4/";
        private HttpClient client = new HttpClient();

        public EddbRepository()
        {
        }

        public async Task<SolarSystem> GetSystem(string name)
        {
            var json = await FetchSystem(name).ConfigureAwait(false);
            return ConvertJsonToSystem(json);
        }

        public async Task<string> FetchSystem(string name)
        {
            var url = baseUrl + "populatedsystems?name=" + HttpUtility.UrlEncode(name);
            return await client.GetStringAsync(url).ConfigureAwait(false);
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
                if(!system.ActiveStates.Any(e => e == state.Name))
                    system.ActiveStates.Add(state.Name);
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
