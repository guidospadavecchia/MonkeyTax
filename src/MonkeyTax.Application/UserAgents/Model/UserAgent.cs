using System.Text.Json.Serialization;

namespace MonkeyTax.Application.UserAgents.Model
{
    public sealed class UserAgent
    {
        [JsonPropertyName("ua")]
        public required string Value { get; set; }
    }
}
