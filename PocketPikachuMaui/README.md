# Pocket Pikachu MAUI - Android App

This is a .NET MAUI Hybrid application that wraps the Pocket Pikachu emulator web app for Android, with native step counter integration.

## Features

- ⚡ **Native Step Counter**: Uses your device's built-in step counter sensor
- 📱 **Runs natively on Android**: Better performance and offline capability
- 🎮 **Full Game Features**: All original emulator features work seamlessly
- 💾 **Persistent Storage**: Your progress is saved locally using Android preferences

## Building the App

### Prerequisites

- .NET 9 SDK
- .NET MAUI workload
- Android SDK (API 24+)
- Visual Studio 2022 or VS Code with C# extensions

### Build Instructions

1. **Install .NET MAUI workload**:
   ```bash
   dotnet workload install maui
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore PocketPikachuMaui/PocketPikachuMaui.csproj
   ```

3. **Build for Android**:
   ```bash
   dotnet build PocketPikachuMaui/PocketPikachuMaui.csproj -f net9.0-android
   ```

4. **Create APK**:
   ```bash
   dotnet publish PocketPikachuMaui/PocketPikachuMaui.csproj \
     -f net9.0-android \
     -c Release \
     -p:AndroidPackageFormats=apk \
     -o ./output
   ```

## Downloading Pre-built APK

The easiest way to get the app is to download the pre-built APK from GitHub Actions:

1. Go to the [Actions tab](../../actions)
2. Click on the latest successful "Build Android APK" workflow run
3. Download the "PocketPikachu-Android-APK" artifact
4. Extract the ZIP and install the APK on your Android device

## Permissions

The app requires the following permission:
- **Activity Recognition** (Android 10+): To access the step counter sensor

## Device Requirements

- Android 7.0 (API 24) or higher
- Device with step counter sensor (most modern smartphones have this)
- ~50 MB of free storage space

## Project Structure

```
PocketPikachuMaui/
├── wwwroot/              # Web assets (HTML, CSS, JS, images)
├── Platforms/Android/    # Android-specific code
│   ├── StepCounterService.cs  # Step counter implementation
│   ├── MainActivity.cs
│   └── AndroidManifest.xml
├── Services/             # Cross-platform services
│   └── StepCounterManager.cs  # Manages step counter integration
├── Interfaces/           # Service interfaces
│   └── IStepCounterService.cs
└── Resources/            # App resources (icons, fonts, etc.)
```

## How It Works

1. The app loads the original JavaScript emulator in a BlazorWebView
2. A background service monitors your device's step counter
3. Every few seconds, the native step count is synchronized with the JavaScript game
4. Steps are simulated by calling the shake button action in JavaScript
5. Your progress is saved using both localStorage (web) and Android Preferences (native)

## Known Limitations

- Background step tracking may be limited by Android battery optimization
- Some devices may not have a step counter sensor (app will work but steps won't auto-increment)
- Alarm feature is basic (future enhancement planned)

## Credits

Based on the [Pocket Pikachu Emulator](https://github.com/Alberto-rp/Pocket-pikachu-emulator) web project.

## License

This is a non-profit fan-made project. All Pokémon-related assets are property of Nintendo/Game Freak/The Pokémon Company.
