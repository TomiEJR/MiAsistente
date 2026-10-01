using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Text;



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

            bool tieneSystemPrompt = _historial.Any(m => m.Role == "system");

            if (!tieneSystemPrompt)  //Esta es la personalidad del modelo en un prompt
            {
                _historial.Insert(0, new ChatMessage
                {
                    Role = "system",
                    Content = "Sos un asistente de voz formal, como un mayordomo. Respondé siempre en español,con respeto, " +
                               "en un máximo de 2 o 3 oraciones cortas, directo al punto, sin rodeos ni " +
                               "explicaciones de más. No uses markdown, asteriscos ni listas."
                });
            }
        }

        public static string QuitarTildes(string texto)
        {
            string normalizado = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in normalizado)
            {
                var categoria = CharUnicodeInfo.GetUnicodeCategory(c);
                if (categoria != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        public async Task<string> AskAsync(string userInput, Func<string, Task> onFraseCompleta)
        {
            _historial.Add(new ChatMessage { Role = "user", Content = userInput });

            var systemMsg = _historial.FirstOrDefault(m => m.Role == "system");
            var sinSystem = _historial.Where(m => m.Role != "system").ToList();

            int cantidadASaltear = Math.Max(0, sinSystem.Count - 4);
            var historialRecortado = sinSystem.Skip(cantidadASaltear).ToList();

            if (systemMsg != null)
                historialRecortado.Insert(0, systemMsg);

            var request = new ChatRequest
            {
                Model = "llama3.2:3b",
                Messages = historialRecortado,
                Stream = true,
                Options = new ChatOptions { NumPredict = 150 }
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
                buffer = buffer + chatResponse.Message.Content;

                if (buffer.EndsWith(".") || buffer.EndsWith("?") || buffer.EndsWith("!") || buffer.EndsWith(":"))
                    {
                    await onFraseCompleta(QuitarTildes(buffer));
                    buffer = "";
                   }


             
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
