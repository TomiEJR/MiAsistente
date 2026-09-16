using System;
using System.Threading.Tasks;
using AsistenteJarvis.Services;

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
            
            #region writelines
            Console.WriteLine("=== JARVIS ===");
            Console.WriteLine("  - 'salir'  termina el programa");
            Console.WriteLine("  - 'escuchar'  activa el micrófono");
            Console.WriteLine();
            #endregion

                while (true)
                {
                    Console.Write("Vos: ");
                    string input = Console.ReadLine()?.Trim();

                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    if (input.ToLower() == "salir")
                        break;

                if (input.ToLower() == "basta")
                    voice.CancelarHabla();

                    if (input.ToLower() == "escuchar")
                    {
                        await grabador.GrabarAsync("grabacion.wav", 5);

                        string texto = await transcriptor.TranscribirAsync("grabacion.wav");

                        input = texto;
                    
                        if (string.IsNullOrWhiteSpace(input))
                        {
                            Console.WriteLine("No te entendí, intentá de nuevo.");
                            continue;
                        }

                        Console.WriteLine($"Escuché: {input}");
                    }

                    string respuesta = await ollama.AskAsync(input);

                    Console.WriteLine($"Jarvis: {respuesta}");

                    voice.Speak(respuesta);
                }

                voice.Dispose();
                Console.WriteLine("Hasta luego.");
            }
        }
}