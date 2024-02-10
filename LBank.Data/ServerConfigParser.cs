using LBank.Domain;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace LBank.Repository
{
    public class ServerConfigParser : IServerConfigParser
    {

        private readonly string _filePath;
        private readonly string _defaultServerName;

        private readonly IFileRepository _fileRepository;

        public ServerConfigParser(IOptions<ServerConfigSettings> configSettings, IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
            _filePath = configSettings.Value.FilePath;
            _defaultServerName = configSettings.Value.DefaultServerName;
        }

        public async Task<IEnumerable<ServerConfig>> ReadServerConfigs()
        {
            var content = await _fileRepository.ReadAllLinesFromFileAsync(_filePath);
            return ReadServerConfigs(content);
        }

        public async Task CreateServerConfig(ServerConfig config)
        {
           await _fileRepository.CreateServerConfigAsync(config, _filePath);
        }

        public async Task<IEnumerable<String>> GetAllServersName()
        {
            var servers = await ReadServerConfigs();
            return servers.Select(sc => sc.ServerName).Distinct().Except(new[] { _defaultServerName }).ToList(); 
        }

        private IEnumerable<ServerConfig> ReadServerConfigs(IEnumerable<string> textLines)
        {

            var configs = new List<ServerConfig>();
            bool isReadingConfig = false;
            ServerConfig? currentConfig = null;
            ServerConfig? defaultConfig = null;
            int counter = 0;

            foreach (var line in textLines)
            {
                switch (line)
                {
                    case string l when l.StartsWith(";START"):
                        isReadingConfig = true;
                        currentConfig = new ServerConfig();
                        break;
                    case string l when l.StartsWith(";END"):
                        SetDefaultConfig(configs, currentConfig, ref defaultConfig, ref counter);
                        isReadingConfig = false;
                        break;
                    default:
                        if (isReadingConfig)
                        {
                            ProcessConfigLine(line, currentConfig, defaultConfig);
                        }
                        break;
                }
            }

            return configs;
        }

        private static void SetDefaultConfig(List<ServerConfig> configs, ServerConfig? currentConfig, ref ServerConfig? defaultConfig, ref int counter)
        {
            if (currentConfig != null)
            {
                configs.Add(currentConfig);
                if (counter++ == 0)
                {
                    defaultConfig = currentConfig;
                }
            }
        }

        private static void ProcessConfigLine(string line, ServerConfig currentConfig, ServerConfig defaultConfig)
        {
            var parts = line.Split(['='], 2);
            if (parts.Length == 2 && currentConfig != null)
            {
                string key = parts[0], value = parts[1].Trim();
                string resolvedName = GetRealServerName(key, out bool isRealName);

                LoadDefaultValues(currentConfig, defaultConfig);

                switch (key)
                {
                    case "SERVER_NAME":
                    case var k when k.StartsWith("SERVER_NAME{"):
                        SetRealServerName(currentConfig, value, resolvedName, isRealName);
                        break;
                    case "URL":
                        currentConfig.Url = value;
                        break;
                    case "DB":
                    case var k when k.StartsWith("DB{"):
                        currentConfig.Db = value;
                        break;
                    case "IP_ADDRESS":
                    case var k when k.StartsWith("IP_ADDRESS{"):
                        currentConfig.IpAddress = value;
                        break;
                    case "DOMAIN":
                    case var k when k.StartsWith("DOMAIN{"):
                        currentConfig.Domain = value;
                        break;
                    case "COOKIE_DOMAIN":
                    case var k when k.StartsWith("COOKIE_DOMAIN{"):
                        currentConfig.CookieDomain = value;
                        break;
                }
            }
        }

        private static void SetRealServerName(ServerConfig currentConfig, string value, string resolvedName, bool isRealName)
        {
            if (isRealName == true)
            {
                currentConfig.ServerName = resolvedName;
            }
            else
            {
                currentConfig.ServerName = value;
            }
        }

        private static void LoadDefaultValues(ServerConfig currentConfig, ServerConfig defaultConfig)
        {
            if (defaultConfig == null) return;

            if (string.IsNullOrEmpty(currentConfig.ServerName) && (defaultConfig?.ServerName != null))
            {
                currentConfig.ServerName = defaultConfig.ServerName;
                currentConfig.Url = defaultConfig.Url;
                currentConfig.Db = defaultConfig.Db;
                currentConfig.IpAddress = defaultConfig.IpAddress;
                currentConfig.Domain = defaultConfig.Domain;
                currentConfig.CookieDomain = defaultConfig.CookieDomain;
            }
        }

        private static string GetRealServerName(string input, out bool isRealName)
        {
            if (!input.Contains("SERVER_NAME"))
            {
                isRealName = false;
                return input;
            }

            string pattern = @"\{(.*?)\}";
            Match match = Regex.Match(input, pattern);
            isRealName = match.Success;
            return isRealName ? match.Groups[1].Value : input;
        }

        public async Task<ServerConfig> GetOneServerConfigs(string serverName)
        {
            var servers = await ReadServerConfigs();
            return servers.FirstOrDefault(sc => sc.ServerName == serverName) ?? new ServerConfig();
        }

        public async Task<bool> UpdateServerConfig(ServerConfig serverConfig)
        {
            return await _fileRepository.UpdateServerConfigAsync(serverConfig, _filePath);
        }
    }
}
