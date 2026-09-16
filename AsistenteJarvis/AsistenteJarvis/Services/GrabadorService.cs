using NAudio.Wave;
using System.Threading.Tasks;

namespace AsistenteJarvis.Services
{
    public class GrabadorService
    {
        public async Task GrabarAsync(string rutaArchivo, int segundos)
        {
            var waveIn = new WaveInEvent();
            var writer = new WaveFileWriter(rutaArchivo, waveIn.WaveFormat);

            // Este evento se dispara cada vez que hay audio nuevo del micrófono
            waveIn.DataAvailable += (sender, e) =>
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);
            };

            waveIn.StartRecording();

            await Task.Delay(segundos * 1000);

            waveIn.StopRecording();

            writer.Dispose();
            waveIn.Dispose();
        }
    }
}