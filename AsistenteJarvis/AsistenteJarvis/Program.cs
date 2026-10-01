using System;
using System.Threading.Tasks;
using AsistenteJarvis.Services;
using System.Diagnostics;


namespace AsistenteJarvis
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var grabador = new GrabadorService();
            var transcriptor = new TranscriptorService();
            var ollama = new OllamaService();
            var voice = new VoiceService();
            var piper = new PiperService();

            Console.WriteLine("=== JARVIS ===");
            Console.WriteLine("  - 'salir'  termina el programa");
            Console.WriteLine("  - 'escuchar'  activa el micrófono");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Vos: ");
                string input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.ToLower() == "salir")
                    break;

                if (input.ToLower() == "escuchar")
                {
                    var sw = Stopwatch.StartNew();
                    await grabador.GrabarAsync("grabacion.wav", 5);
                    Console.WriteLine($"[Grabar: {sw.ElapsedMilliseconds} ms]");

                    sw.Restart();
                    string texto = await transcriptor.TranscribirAsync("grabacion.wav");
                    Console.WriteLine($"[Transcribir: {sw.ElapsedMilliseconds} ms]");

                    input = texto;

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("No te entendí, intentá de nuevo.");
                        continue;
                    }

                    Console.WriteLine($"Escuché: {input}");
                }

                var swOllama = Stopwatch.StartNew();
                string respuesta = await ollama.AskAsync(input, async frase => await piper.HablarAsync(frase));
                Console.WriteLine($"[Ollama: {swOllama.ElapsedMilliseconds} ms]");

               // Console.WriteLine($"Jarvis: {respuesta}");
              //  voice.Speak(respuesta);
            }
        }
    }
}