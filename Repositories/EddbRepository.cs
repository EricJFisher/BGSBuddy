using Interfaces.Repositories;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Repositories
{
    public class EddbRepository : IEddbRepository
    {
        private readonly string baseUrl = "https://eddbapi.kodeblox.com/api/v4/";
        private HttpClient client = new HttpClient();

        public EddbRepository()
        {
        }

        public async Task<List<string>> GetHomeFactions(string systemName)
        {
            var json = await FetchHomeFactions(systemName).ConfigureAwait(false);
            return ConvertJsonToFactions(json);
        }

        public async Task<string> FetchHomeFactions(string systemName)
        {
            var url = baseUrl + "factions?homesystemname=" + HttpUtility.UrlEncode(systemName);
            return await client.GetStringAsync(url).ConfigureAwait(false);
        }

        private List<string> ConvertJsonToFactions(string json)
        {
            JObject o = JObject.Parse(json);
            var factions = o.Descendants()
            .Where(t => t.Type == JTokenType.Property && ((JProperty)t).Name == "name_lower")
            .Select(p => (string)((JProperty)p).Value).ToList();

            return factions;
        }
    }
}
