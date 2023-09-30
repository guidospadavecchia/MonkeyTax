namespace MonkeyTax.Application.UserAgents.Services
{
    public interface IUserAgentService
    {
        Task<string?> GetRandomUserAgentAsync(CancellationToken cancellationToken = default);
    }
}
