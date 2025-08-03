using System;
using System.IO;
using System.Text.RegularExpressions;
using NAudio.Wave;
using Vosk;
using WindowsInput;
using WindowsInput.Native;

class Program
{
    static void Main()
    {
        Vosk.Vosk.SetLogLevel(0);
        const string modelPath = "vosk-model-small-de-0.15";
        var model = new Model(modelPath);

        using var recognizer = new VoskRecognizer(model, 16000.0f);
        using var waveIn = new WaveInEvent();
        var sim = new InputSimulator();
        waveIn.DeviceNumber = 0;
        waveIn.WaveFormat = new WaveFormat(16000, 1);
        waveIn.DataAvailable += (s, a) =>
        {
            if (recognizer.AcceptWaveform(a.Buffer, a.BytesRecorded))
            {
                var text = recognizer.Result().ToLower();
                Console.WriteLine(text);
                foreach (var acceptedSentance in AcceptedSentences)
                {
                    if (text.Contains(acceptedSentance))
                    {
                        Console.WriteLine("🎬 Trigger erkannt: Video speichern!");
                        sim.Keyboard.KeyPress(VirtualKeyCode.F8);
                        break;
                    }
                }
            }
        };

        Console.WriteLine("Sprich jetzt... (Enter zum Beenden)");
        waveIn.StartRecording();
        Console.ReadLine();
        waveIn.StopRecording();
    }

    public static List<string> AcceptedSentences = new List<string>()
    {
        "video speichern",
        "videos speichern",
    };
}