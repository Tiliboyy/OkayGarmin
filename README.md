# 🎤 Mic-to-Text Hotkey Trigger (Vosk + NAudio + C#)

Dieses kleine C#-Konsolenprogramm nutzt **Vosk** zur Spracherkennung und **NAudio**, um über das Mikrofon zuzuhören. Sobald eines der definierten Sprachkommandos erkannt wird, wird eine Tastenkombination (z. B. F8) ausgelöst – ideal zum Beispiel für „Video speichern“-Hotkeys.

## 🚀 Features

- Echtzeit-Spracherkennung mit Vosk
- Unterstützt deutsche Sprache
- Löst Hotkey bei bestimmten Sprachbefehlen aus
- Minimaler Ressourcenbedarf

## 📦 Setup

1. **Vosk-Modell herunterladen**:  
   Lade das deutsche Sprachmodell von Vosk herunter:  
   👉 [vosk-model-small-de-0.15](https://alphacephei.com/vosk/models)

2. **Modell entpacken** und in den Projektordner legen:  
   ```bash
   vosk-model-small-de-0.15/
   ├── am
   ├── conf
   └── ...
