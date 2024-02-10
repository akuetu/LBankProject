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
          await  _serverConfigParser.CreateServerConfig(serverConfig);           
        }

        public async Task<IEnumerable<string>> GetAllServersName()
        {
           return await _serverConfigParser.GetAllServersName();
        }

         
    }

}
