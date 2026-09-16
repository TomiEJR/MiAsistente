using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace AsistenteJarvis
{
    public class TranscriptorService
    {
        public async Task<string> TranscribirAsync(string rutaAudio)
        {
            string rutaScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Transcriptor.py");
            var psi = new ProcessStartInfo
            {
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                FileName = "python",
                Arguments = $"\"{rutaScript}\" \"{rutaAudio}\"",
                RedirectStandardOutput = true,   // queremos leer lo que imprime
                UseShellExecute = false,          // necesario para poder redirigir
                CreateNoWindow = true             //  no abrir una ventana de consola nueva
            };

            using (var process = Process.Start(psi))
            {
                string resultado = await process.StandardOutput.ReadToEndAsync();
                process.WaitForExit();
                return resultado.Trim();
            }
        }
    }
}
