using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;

namespace AsistenteJarvis.Services
{

    public class OllamaService
    {
        HttpClient _http;
       
        private List<ChatMessage> _historial;
        public OllamaService()
        {
            _http = new HttpClient();
            _http.BaseAddress = new Uri("http://localhost:11434");

            if (File.Exists("historial.json"))
            {
                string json = File.ReadAllText("historial.json");
                _historial = JsonConvert.DeserializeObject<List<ChatMessage>>(json);
            }
            else
            {
                _historial = new List<ChatMessage>();
            }
        }

        public async Task<string> AskAsync(string userInput)
        {

            _historial.Add(new ChatMessage { Role = "user", Content = userInput });

            var request = new ChatRequest
            {
                Model = "llama3.2:3b",
                Messages = _historial,
                Stream = false
            };
         
            string json = JsonConvert.SerializeObject(request);
            
            var contenido = new StringContent(json, Encoding.UTF8, "application/json"); //Pone una etiqueta para que el servidor pueda interpretarlo correctamente como json

            var response = await _http.PostAsync("/api/chat", contenido); //Manda la peticion para la respuesta de ollama

            string responseBody = await response.Content.ReadAsStringAsync(); //Esta es la respuesta en si, lee "response" como string

            var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(responseBody);//Convierte "responseBody" a un objeto ChatResponse.cs y asisgna los campos


           

            _historial.Add(new ChatMessage { Role = "assistant", Content = chatResponse.Message.Content });

            string loadjson = JsonConvert.SerializeObject(_historial);
            File.WriteAllText("historial.json", loadjson);


            return chatResponse.Message.Content;



        }

    }
}
