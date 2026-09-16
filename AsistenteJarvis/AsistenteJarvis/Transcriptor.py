from faster_whisper import WhisperModel
import sys

model = WhisperModel("base", device="cpu", compute_type="int8")


archivo_audio = sys.argv[1]
segments, info = model.transcribe(archivo_audio, language="es")

texto = ""
for segment in segments:
    texto += segment.text + " "

print(texto.strip())