using LBank.Domain;

namespace LBank.Business
{
    public interface IServerConfigService
    {
        public Task<IEnumerable<ServerConfig>> ReadServerConfigs();
        public Task<IEnumerable<String>> GetAllServersName();
        public Task CreateServerConfig(ServerConfig config);
    }
}
