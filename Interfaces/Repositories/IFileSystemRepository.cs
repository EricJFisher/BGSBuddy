namespace Interfaces.Repositories
{
    public interface IFileSystemRepository
    {
        void SaveJsonToFile(string json, string fileName);
        string RetrieveJsonFromFile(string fileName);
    }
}
