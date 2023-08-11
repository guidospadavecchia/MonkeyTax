using Microsoft.Extensions.Caching.Memory;
using MonkeyTax.Application.UserAgents.Model;
using RestSharp;
using System.Text.Json;

namespace MonkeyTax.Application.UserAgents.Services
{
    public class UserAgentService : IUserAgentService
    {
        private const string CACHE_KEY = "UserAgents";
        private readonly TimeSpan _absoluteExpiration = TimeSpan.FromDays(30);

        private static readonly Random _random = new();

        private readonly RestClient? _client;
        private readonly IMemoryCache _memoryCache;

        public UserAgentService(string? url, IMemoryCache memoryCache)
        {
            _client = !string.IsNullOrWhiteSpace(url) ? new RestClient(url) : null;
            _memoryCache = memoryCache;
        }

        public async Task<string?> GetRandomUserAgent(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_memoryCache.TryGetValue(CACHE_KEY, out string[] cachedUserAgents))
                {
                    int randomIndex = _random.Next(0, cachedUserAgents.Length);
                    return cachedUserAgents[randomIndex];
                }

                if (_client != null)
                {
                    RestRequest request = new();
                    RestResponse response = await _client.GetAsync(request, cancellationToken);
                    if (response != null && response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
                    {
                        UserAgentsCollection? userAgents = JsonSerializer.Deserialize<UserAgentsCollection>(response.Content);
                        if (userAgents != null)
                        {
                            string[] agents = userAgents.Collection.Select(x => x.Value).ToArray();
                            _memoryCache.Set(CACHE_KEY, agents, _absoluteExpiration);

                            int randomIndex = _random.Next(0, agents.Length);
                            string randomAgent = agents[randomIndex];
                            return randomAgent;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user agents: {ex.Message}");
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}
