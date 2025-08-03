using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using NAudio.Wave;
using Vosk;
using WindowsInput;
using WindowsInput.Native;

class Config
{
    public string ModelPath { get; set; } = "vosk-model-small-de-0.15";
    public List<string> AcceptedSentences { get; set; } = new List<string> { "video speichern", "videos speichern" };
    public string Hotkey { get; set; } = "F8";
    public int MicDeviceIndex { get; set; } = 0;
    public int SampleRate { get; set; } = 16000;
}

class Program
{
    static void Main()
    {
        const string configPath = "config.json";
        var config = LoadOrCreateConfig(configPath);

        Vosk.Vosk.SetLogLevel(0);
        var model = new Model(config.ModelPath);

        using var recognizer = new VoskRecognizer(model, config.SampleRate);
        using var waveIn = new WaveInEvent();
        var sim = new InputSimulator();

        waveIn.DeviceNumber = config.MicDeviceIndex;
        waveIn.WaveFormat = new WaveFormat(config.SampleRate, 1);
        var hotkeyParsed = ParseHotkey(config.Hotkey);

        waveIn.DataAvailable += (s, a) =>
        {
            if (recognizer.AcceptWaveform(a.Buffer, a.BytesRecorded))
            {
                var text = recognizer.Result().ToLower();
                Console.WriteLine(text);

                foreach (var trigger in config.AcceptedSentences)
                {
                    if (text.Contains(trigger))
                    {
                        Console.WriteLine($"🎬 Trigger erkannt: '{trigger}' → {config.Hotkey}");
                        sim.Keyboard.KeyPress(hotkeyParsed);
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

    static Config LoadOrCreateConfig(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("⚠️  Konfigurationsdatei nicht gefunden. Erstelle 'config.json' mit Standardwerten...");
            var defaultConfig = new Config();
            var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
            Console.WriteLine("✅ Datei 'config.json' wurde erstellt. Bitte ggf. anpassen und erneut starten.");
            Environment.Exit(0);
        }

        var fileContent = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Config>(fileContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    static VirtualKeyCode ParseHotkey(string hotkey)
    {
        if (Enum.TryParse<VirtualKeyCode>(hotkey.ToUpper(), out var code))
            return code;

        Console.WriteLine($"❌ Ungültiger Hotkey: '{hotkey}'");
        Environment.Exit(1);
        return 0;
    }
}
