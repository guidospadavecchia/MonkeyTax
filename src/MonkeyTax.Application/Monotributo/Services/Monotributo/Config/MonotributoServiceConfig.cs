namespace MonkeyTax.Application.Monotributo.Services.Monotributo.Config
{
    public sealed class MonotributoServiceConfig
    {
        public required string MonotributoUrl { get; set; }
        public required Dictionary<string, string> Headers { get; set; }
    }
}
