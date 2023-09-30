using RestSharp;
using System.Net;

namespace MonkeyTax.Application.Proxies.Services
{
    public class ProxyService : IProxyService
    {
        private readonly RestClient? _client;

        public ProxyService(string? url)
        {
            _client = !string.IsNullOrWhiteSpace(url) ? new RestClient(url) : null;
        }

        public async Task<WebProxy?> GetRandomProxyAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_client != null)
                {
                    RestRequest request = new();
                    RestResponse response = await _client.GetAsync(request, cancellationToken);
                    if (response != null && response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
                    {
                        if (Uri.TryCreate(response.Content, UriKind.Absolute, out Uri? uri) && uri != null)
                        {
                            return new(uri);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving proxy: {ex.Message}");
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}
