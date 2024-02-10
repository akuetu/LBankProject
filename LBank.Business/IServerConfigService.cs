using LBank.Domain;

namespace LBank.Business
{
    public interface IServerConfigService
    {
        public Task<IEnumerable<ServerConfig>> ReadServerConfigs();

        public Task<IEnumerable<String>> GetAllServersName();

        public Task CreateServerConfig(ServerConfig serverConfig);

        public  Task<ServerConfig> GetOneServerConfigs(string serverName);

        public Task<bool> UpdateServerConfig(ServerConfig serverConfig);
    }
}
