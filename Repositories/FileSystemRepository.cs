using System;
using System.IO;
using System.Threading.Tasks;

namespace Repositories
{
    public class FileSystemRepository
    {
        private string logPath;

        public FileSystemRepository()
        {
            logPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "BGSBuddy" + Path.DirectorySeparatorChar; ;
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
        }

        public async Task SaveJsonToFile(string json, string fileName)
        {
            File.WriteAllText(fileName, json);
        }

        public async Task<string> RetrieveJsonFromFile(string fileName)
        {
            return File.ReadAllText(fileName);
        }
    }
}
