using LBank.Domain;
using LBank.Repository;

namespace LBank.Business
{
    public class ServerConfigService : IServerConfigService
    {
        private readonly IServerConfigParser _serverConfigParser;

        public ServerConfigService(IServerConfigParser serverConfigParser)
        {
            _serverConfigParser = serverConfigParser;
        }

        public async Task<IEnumerable<ServerConfig>> ReadServerConfigs()
        {
           return await _serverConfigParser.ReadServerConfigs();            
        }

        public async Task CreateServerConfig(ServerConfig serverConfig)
        {
            ArgumentNullException.ThrowIfNull(serverConfig);
            await  _serverConfigParser.CreateServerConfig(serverConfig);           
        }

        public async Task<IEnumerable<string>> GetAllServersName()
        {
           return await _serverConfigParser.GetAllServersName();
        }

        public async Task<ServerConfig> GetOneServerConfigs(string serverName)
        {
            return await _serverConfigParser.GetOneServerConfigs(serverName);
        }

        public async Task<bool> UpdateServerConfig(ServerConfig serverConfig)
        {
            return serverConfig == null
                ? throw new ArgumentNullException(nameof(serverConfig))
                : await _serverConfigParser.UpdateServerConfig(serverConfig);
        }
    }

}
