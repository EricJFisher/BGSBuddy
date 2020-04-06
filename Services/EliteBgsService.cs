using Repositories;
using Services.QuickType;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Services
{
    public class EliteBgsService
    {
        private readonly string logPath;
        private EliteBgsRepository repository = new EliteBgsRepository();

        public EliteBgsService()
        {
            logPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "BGSBuddy" + Path.DirectorySeparatorChar; ;
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
        }

        public async Task GetFaction(string name)
        {
            await repository.FetchFaction(name);
            var file = logPath + "F" + name + ".log";

            var json = File.ReadAllText(file);
            var result = EliteBgs.FromJson(json);
        }
    }
}
