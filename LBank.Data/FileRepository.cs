using LBank.Domain;
using System.Text.RegularExpressions;

namespace LBank.Repository
{
    public class FileRepository : IFileRepository
    {
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public async Task<IEnumerable<string>> ReadAllLinesFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<string>();
            }
            await semaphore.WaitAsync();
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                return lines;
            }
            finally
            {
                semaphore.Release();
            }
        }

        //make async
        public void Create(ServerConfig config, string filePath)
        {
            var configs = ReadAll(filePath);
            if (configs.Any(c => c.ServerName == config.ServerName))
            {
                throw new Exception("A config with the same ServerName already exists.");
            }
            // SMAPHERO ASYNC
            using (var sw = File.AppendText(filePath))
            {
                sw.WriteLine($";START {config.ServerName}");
                sw.WriteLine($"SERVER_NAME{{{config.ServerName}}}={config.ServerName}");
                sw.WriteLine($"URL{{{config.ServerName}}}={config.Url}");
                sw.WriteLine($"DB{{{config.ServerName}}}={config.Db}");
                sw.WriteLine($"IP_ADDRESS{{{config.ServerName}}}={config.IpAddress}");
                sw.WriteLine($"DOMAIN{{{config.ServerName}}}={config.Domain}");
                sw.WriteLine($"COOKIE_DOMAIN{{{config.ServerName}}}={config.CookieDomain}");
                sw.WriteLine($";END {config.ServerName}");
                sw.WriteLine();
            }
        }

        //make async
        public static List<ServerConfig> ReadAll(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<ServerConfig>();
            }
            // SMAPHERO ASYNC
            var fileContent = File.ReadAllText(filePath);
            var matches = Regex.Matches(fileContent, @";START (.+?)(SERVER_NAME\{.+?\}=(.+?)\n)?(URL\{.+?\}=(.+?)\n)?(DB\{.+?\}=(.+?)\n)?(IP_ADDRESS\{.+?\}=(.+?)\n)?(DOMAIN\{.+?\}=(.+?)\n)?(COOKIE_DOMAIN\{.+?\}=(.+?)\n)?;END .+?", RegexOptions.Singleline);

            var configs = new List<ServerConfig>();
            foreach (Match match in matches)
            {
                var config = new ServerConfig
                {
                    ServerName = match.Groups[3].Value,
                    Url = match.Groups[5].Value,
                    Db = match.Groups[7].Value,
                    IpAddress = match.Groups[9].Value,
                    Domain = match.Groups[11].Value,
                    CookieDomain = match.Groups[13].Value
                };
                configs.Add(config);
            }

            return configs;
        }

        //public static void Update(string serverName, ServerConfig updatedConfig, string filePath)
        //{
        //    var configs = ReadAll(filePath);
        //    var configIndex = configs.FindIndex(c => c.ServerName == serverName);
        //    if (configIndex == -1)
        //    {
        //        throw new Exception("Config not found for update.");
        //    }

        //    configs[configIndex] = updatedConfig;
        //    SaveAll(configs, filePath);
        //}

        //public static void Delete(string serverName, string filePath)
        //{
        //    var configs = ReadAll(filePath);
        //    if (configs.RemoveAll(c => c.ServerName == serverName) > 0)
        //    {
        //        SaveAll(configs, filePath);
        //    }
        //}

        //make async
        private static void SaveAll(List<ServerConfig> configs, string filePath)
        {
            using (var sw = new StreamWriter(filePath, false))
            {
                foreach (var config in configs)
                {
                    sw.WriteLine($";START {config.ServerName}");
                    sw.WriteLine($"SERVER_NAME{{{config.ServerName}}}={config.ServerName}");
                    sw.WriteLine($"URL{{{config.ServerName}}}={config.Url}");
                    sw.WriteLine($"DB{{{config.ServerName}}}={config.Db}");
                    sw.WriteLine($"IP_ADDRESS{{{config.ServerName}}}={config.IpAddress}");
                    sw.WriteLine($"DOMAIN{{{config.ServerName}}}={config.Domain}");
                    sw.WriteLine($"COOKIE_DOMAIN{{{config.ServerName}}}={config.CookieDomain}");
                    sw.WriteLine($";END {config.ServerName}");
                }
            }
        }
    }
}
