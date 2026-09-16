using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AsistenteJarvis.Services
{
    public class ChatResponse
    {
        [JsonProperty("message")]
        public ChatMessage Message { get; set; }


        [JsonProperty("done")]
        public bool Done { get; set; }  

    }
}
