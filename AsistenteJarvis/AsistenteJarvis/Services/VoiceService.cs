using System;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Globalization;

namespace AsistenteJarvis.Services
{
    public class VoiceService : IDisposable
    {
        private SpeechSynthesizer _synthesizer;
        private SpeechRecognitionEngine _recognizer;

        public VoiceService()
        {
            _synthesizer = new SpeechSynthesizer();
            _synthesizer.SetOutputToDefaultAudioDevice();
            _synthesizer.Rate = 0;      
            _synthesizer.Volume = 100;  

            foreach (InstalledVoice voice in _synthesizer.GetInstalledVoices()) //Busca si tiene voz en español
            {
                if (voice.VoiceInfo.Culture.Name.StartsWith("es"))
                {
                    _synthesizer.SelectVoice(voice.VoiceInfo.Name);
                    break;
                }
            }

            foreach (var r in SpeechRecognitionEngine.InstalledRecognizers())
            {
                Console.WriteLine($"{r.Culture} - {r.Name}");
            }
            _recognizer = new SpeechRecognitionEngine(new CultureInfo("es-ES")); //Escucha mi microfono

            _recognizer.LoadGrammar(new DictationGrammar()); //Reconoce mis palabras

            _recognizer.SetInputToDefaultAudioDevice(); //Usa el microfono predeterminado
        }

        public void Speak(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            _synthesizer.SpeakAsyncCancelAll(); // Cancela lo que estaba diciendo
    _synthesizer.SpeakAsync(text);      // Habla el texto nuevo
        }

        public void NewSpeak(string text)
        {
            _synthesizer.Speak(text);
        }


        public void CancelarHabla()
        {
            _synthesizer.SpeakAsyncCancelAll();
        }
        public string Listen()
        {
            Console.WriteLine("Escuchando... (hablá ahora)");

            RecognitionResult result = _recognizer.Recognize();

            if (result != null && !string.IsNullOrWhiteSpace(result.Text))
            {
                return result.Text;
            }

            return null;
        }

        public void Dispose()
        {
            _synthesizer?.Dispose();
            _recognizer?.Dispose();
        }
    }
}