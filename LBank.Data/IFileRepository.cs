using LBank.Domain;

namespace LBank.Repository
{
    public interface IFileRepository
    {
        public Task<IEnumerable<string>> ReadAllLinesFromFileAsync(string filePath);

        public Task CreateServerConfigAsync(ServerConfig config, string filePath);

        Task<bool> UpdateServerConfigAsync(ServerConfig updatedConfig, string filePath);
    }
}
