using LBank.Domain;
using System.Text.RegularExpressions;

namespace LBank.Repository
{
    public class FileRepository : IFileRepository
    {
        private static readonly SemaphoreSlim ReadAllSemaphore = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim CreateSemaphore = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim ReadSemaphore = new SemaphoreSlim(1, 1);


        public async Task<IEnumerable<string>> ReadAllLinesFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<string>();
            }
            await ReadAllSemaphore.WaitAsync();
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                return lines;
            }
            finally
            {
                ReadAllSemaphore.Release();
            }
        }         

        public async Task CreateServerConfigAsync(ServerConfig config, string filePath)
        {
            var configs = await ReadAllAsync(filePath);  
            if (configs.Any(c => c.ServerName == config.ServerName))
            {
                throw new Exception("A config with the same ServerName already exists.");
            }

            await CreateSemaphore.WaitAsync();
            try
            {
                await using (var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                using (var sw = new StreamWriter(stream))
                {
                    await sw.WriteLineAsync($";START {config.ServerName}");
                    await sw.WriteLineAsync($"SERVER_NAME{{{config.ServerName}}}={config.ServerName}");
                    await sw.WriteLineAsync($"URL{{{config.ServerName}}}={config.Url}");
                    await sw.WriteLineAsync($"DB{{{config.ServerName}}}={config.Db}");
                    await sw.WriteLineAsync($"IP_ADDRESS{{{config.ServerName}}}={config.IpAddress}");
                    await sw.WriteLineAsync($"DOMAIN{{{config.ServerName}}}={config.Domain}");
                    await sw.WriteLineAsync($"COOKIE_DOMAIN{{{config.ServerName}}}={config.CookieDomain}");
                    await sw.WriteLineAsync($";END {config.ServerName}");
                    await sw.WriteLineAsync(); 
                }
            }
            finally
            {
                CreateSemaphore.Release();
            }
        }
    
        public static async Task<List<ServerConfig>> ReadAllAsync(string filePath)
        {
            await ReadSemaphore.WaitAsync();
            try
            {
                if (!File.Exists(filePath))
                {
                    return new List<ServerConfig>();
                }

                // Asynchronously read the entire file content
                var fileContent = await File.ReadAllTextAsync(filePath);
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
            finally
            {
                ReadSemaphore.Release();
            }
        }         
    }
}
