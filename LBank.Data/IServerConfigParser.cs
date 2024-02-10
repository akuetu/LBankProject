﻿using LBank.Domain;

namespace LBank.Repository
{
    public interface IServerConfigParser
    {
        public Task<IEnumerable<ServerConfig>> ReadServerConfigs();

        public Task<IEnumerable<String>> GetAllServersName();

        public Task CreateServerConfig(ServerConfig config);

        public Task<ServerConfig> GetOneServerConfigs(string serverName);
       
       public Task<bool> UpdateServerConfig(ServerConfig serverConfig);
    }
}
