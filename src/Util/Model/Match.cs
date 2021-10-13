using System.Text.Json.Serialization;

namespace Util.Model
{
    public class Match
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        public string MasterPlayerEntityId { get; set; }

        public bool IsMatchWon { get; set; }
    }
}