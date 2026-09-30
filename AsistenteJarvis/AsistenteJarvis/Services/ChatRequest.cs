using System.Collections.Generic;
using Newtonsoft.Json;

namespace AsistenteJarvis.Services
{
    public class ChatRequest
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("messages")]
        public List<ChatMessage> Messages { get; set; }

        [JsonProperty("stream")]
        public bool Stream { get; set; }

        [JsonProperty("options")]
        public ChatOptions Options { get; set; }
    }
}