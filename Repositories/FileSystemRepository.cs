using Interfaces.Repositories;
using System;
using System.IO;

namespace Repositories
{
    public class FileSystemRepository : IFileSystemRepository
    {
        private readonly string _logPath;

        public FileSystemRepository()
        {
            _logPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "BGSBuddy" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(_logPath))
                Directory.CreateDirectory(_logPath);
        }

        public void SaveJsonToFile(string json, string fileName)
        {
            File.WriteAllText(_logPath + fileName, json);
        }

        public string RetrieveJsonFromFile(string fileName)
        {
            if (File.Exists(_logPath + fileName))
                return File.ReadAllText(_logPath + fileName);
            else
                return string.Empty;
        }
    }
}
