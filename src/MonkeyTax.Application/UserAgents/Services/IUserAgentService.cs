namespace MonkeyTax.Application.UserAgents.Services
{
    public interface IUserAgentService
    {
        Task<string?> GetRandomUserAgent(CancellationToken cancellationToken = default);
    }
}
