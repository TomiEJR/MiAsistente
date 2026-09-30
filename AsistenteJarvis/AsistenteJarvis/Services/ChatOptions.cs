using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace AsistenteJarvis.Services
{
   public class ChatOptions
    {
        [JsonProperty("num_predict")]
      public  int NumPredict { get; set; }
}
}
