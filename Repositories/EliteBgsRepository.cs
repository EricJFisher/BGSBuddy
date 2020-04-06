using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Repositories
{
    public class EliteBgsRepository
    {
        private readonly string logPath;
        private readonly string baseUrl = "https://elitebgs.app/api/ebgs/v4/";
        private HttpClient client = new HttpClient();

        public EliteBgsRepository()
        {
            logPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "BGSBuddy" + Path.DirectorySeparatorChar; ;
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
        }

        public async Task FetchFaction(string name)
        {
            var file = logPath + "F" + name + ".log";
            if (File.GetLastWriteTime(file) > DateTime.Now.Date)
                return;
            var url = baseUrl + "factions?name=" + name;
            var response = await client.GetStringAsync(url);
            File.WriteAllText(file, response);
        }

        public async Task FetchSystem(string name)
        {
            var file = logPath + "S" + name + ".log";
            if (File.GetLastWriteTime(file) > DateTime.Now.Date)
                return;
            var url = baseUrl + "systems?name=" + name;
            var response = await client.GetStringAsync(url);
            File.WriteAllText(file, response);
        }

        public async Task FetchStation(string name)
        {
            var file = logPath + "A" + name + ".log";
            if (File.GetLastWriteTime(file) > DateTime.Now.Date)
                return;
            var url = baseUrl + "stations?name=" + name;
            var response = await client.GetStringAsync(url);
            File.WriteAllText(file, response);
        }
    }
}
