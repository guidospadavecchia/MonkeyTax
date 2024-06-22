using System.Text.Json.Serialization;

namespace MonkeyTax.Application.UserAgents.Model
{
    public sealed class UserAgentsCollection
    {
        [JsonPropertyName("data")]
        public IEnumerable<UserAgent> Collection { get; set; } = [];
    }
}
