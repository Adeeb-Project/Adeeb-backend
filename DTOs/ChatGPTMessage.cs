using System.Text.Json.Serialization;

namespace AdeebBackend.DTOs
{
    public class ChatGPTMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }
        
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
