using MonkeyTax.Application.Monotributo.Model;

namespace MonkeyTax.Application.Monotributo.Services.Monotributo
{
    public interface IMonotributoService
    {
        Task<MonotributoResponse> GetValuesAsync(string? cacheControlHeader, CancellationToken cancellationToken = default);
    }
}
