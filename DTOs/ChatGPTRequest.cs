using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AdeebBackend.DTOs
{
    public class ChatGPTRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }
        
        [JsonPropertyName("messages")]
        public List<ChatGPTMessage> Messages { get; set; }
    }
}
