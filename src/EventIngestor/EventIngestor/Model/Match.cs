using System.Text.Json.Serialization;

namespace EventIngestor.Model
{
    public class Match
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        public string PlayerEntityId { get; set; }

        public bool IsMatchWon { get; set; }
    }
}