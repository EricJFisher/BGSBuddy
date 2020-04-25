using System.Net.Http;
using System.Threading.Tasks;

namespace Repositories
{
    public static class GithubRepository
    {
        public static async Task<bool> HasUpdates(string currentVersion)
        {
            var baseUrl = @"https://api.github.com/repos/EricJFisher/BGSBuddy/releases/latest";
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");
            var response = await client.GetStringAsync(baseUrl).ConfigureAwait(false);
            return !response.Contains(currentVersion);
        }
    }
}
