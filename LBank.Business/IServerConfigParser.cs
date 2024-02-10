using LBank.Domain;

namespace LBank.Business
{
    public interface IServerConfigParser
    {
        public Task<IEnumerable<ServerConfig>> ReadServerConfigs();
        public void Create(ServerConfig config);
    }
}
