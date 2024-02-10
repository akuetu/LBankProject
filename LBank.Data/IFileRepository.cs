using LBank.Domain;

namespace LBank.Repository
{
    public interface IFileRepository
    {
        public Task<IEnumerable<string>> ReadAllLinesFromFileAsync(string filePath);

        public void Create(ServerConfig config, string filePath);
    }
}
