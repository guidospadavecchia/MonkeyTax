using System.Net;

namespace MonkeyTax.Application.Proxies.Services
{
    public interface IProxyService
    {
        Task<WebProxy?> GetRandomProxyAsync(CancellationToken cancellationToken = default);
    }
}
