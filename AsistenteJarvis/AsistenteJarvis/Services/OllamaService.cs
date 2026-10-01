using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;


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

            int cantidadASaltear = Math.Max(0, _historial.Count - 4);
            var historialRecortado = _historial.Skip(cantidadASaltear).ToList();

            var request = new ChatRequest
            {
                Model = "llama3.2:3b",
                Messages = historialRecortado,
                Stream = true,
                Options = new ChatOptions { NumPredict = 1000 }
            };

            var sw = Stopwatch.StartNew();
            string json = JsonConvert.SerializeObject(request);
            Console.WriteLine($"[Serializar: {sw.ElapsedMilliseconds} ms] [Tamaño JSON: {json.Length} caracteres]");

            var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            sw.Restart();
            var request2 = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
            {
                Content = contenido
            };

            var response = await _http.SendAsync(request2, HttpCompletionOption.ResponseHeadersRead);
            Console.WriteLine($"[PostAsync (HTTP real): {sw.ElapsedMilliseconds} ms]");

            var stream = await response.Content.ReadAsStreamAsync();
            var reader = new StreamReader(stream);
            var textoCompleto = new StringBuilder();

            string buffer = "";
            while (!reader.EndOfStream)
            {

             string Leido= await reader.ReadLineAsync();

                var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(Leido);

                Console.Write(chatResponse.Message.Content);
                if (chatResponse.Done == true) break;
              //  onFraseCompleta("texto de la oración completa");

                textoCompleto.Append(chatResponse.Message.Content);


             
            }


            _historial.Add(new ChatMessage { Role = "assistant", Content = textoCompleto.ToString()});

            string loadjson = JsonConvert.SerializeObject(_historial);

            File.WriteAllText("historial.json", loadjson);
            Console.WriteLine($"[Guardar archivo: {sw.ElapsedMilliseconds} ms]");
         
            #region NoActivar
            /*            sw.Restart();
            string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"[Leer respuesta: {sw.ElapsedMilliseconds} ms]");

                        var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(responseBody);

                        _historial.Add(new ChatMessage { Role = "assistant", Content = chatResponse.Message.Content });

                        sw.Restart();
                        string loadjson = JsonConvert.SerializeObject(_historial);
                        File.WriteAllText("historial.json", loadjson);
                        Console.WriteLine($"[Guardar archivo: {sw.ElapsedMilliseconds} ms]");

                        return chatResponse.Message.Content;*/
                #endregion

            return textoCompleto.ToString();
        }

    }
}
