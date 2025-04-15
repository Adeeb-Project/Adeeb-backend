using System.Text.Json.Serialization;

namespace AdeebBackend.DTOs
{
    public class ChatGPTChoice
    {
        [JsonPropertyName("message")]
        public ChatGPTMessage Message { get; set; }
    }
}
