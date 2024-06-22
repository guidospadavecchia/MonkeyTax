namespace MonkeyTax.Application.Monotributo.Services.Monotributo.Config
{
    public sealed class MonotributoServiceConfig
    {
        public string MonotributoUrl { get; set; } = null!;
        public Dictionary<string, string> Headers { get; set; } = [];
    }
}
