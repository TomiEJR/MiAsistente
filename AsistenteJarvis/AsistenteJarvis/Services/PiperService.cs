using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AsistenteJarvis.Services
{
    public class PiperService
    {
        public async Task HablarAsync(string texto)
        {
            string rutaPiper = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Piper", "piper", "piper.exe");
            string rutaModelo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Piper", "piper", "es_ES-sharvard-medium.onnx");
            string rutaSalida = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "respuesta.wav");

            var psi = new ProcessStartInfo
            {
                FileName = rutaPiper,
                Arguments = $"--model \"{rutaModelo}\" --output_file \"{rutaSalida}\"",
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var sw = Stopwatch.StartNew();

            using (var process = Process.Start(psi))
            {
                await process.StandardInput.WriteLineAsync(texto);
                process.StandardInput.Close();
                process.WaitForExit();
            }

            Console.WriteLine($"[Piper generar audio: {sw.ElapsedMilliseconds} ms]");

            sw.Restart();
            ReproducirWav(rutaSalida);
            Console.WriteLine($"[Reproducir: {sw.ElapsedMilliseconds} ms]");
        }
        private void ReproducirWav(string rutaWav)
        {
            using (var reader = new NAudio.Wave.AudioFileReader(rutaWav))
            using (var output = new NAudio.Wave.WaveOutEvent())
            {
                output.Init(reader);
                output.Play();

                while (output.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    System.Threading.Thread.Sleep(1); // esperamos a que termine de sonar
                }
            }
        }
    }
}