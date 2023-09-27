using System.Text.Json.Serialization;

namespace MonkeyTax.Application.UserAgents.Model
{
    public sealed class UserAgent
    {
        [JsonPropertyName("ua")]
        public string Value { get; set; } = null!;
    }
}
