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
            logPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "BGSBuddy" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
        }

        public async Task SaveJsonToFile(string json, string fileName)
        {
            File.WriteAllText(logPath + fileName, json);
        }

        public async Task<string> RetrieveJsonFromFile(string fileName)
        {
            if (File.Exists(logPath + fileName))
                return File.ReadAllText(logPath + fileName);
            else
                return string.Empty;
        }
    }
}
