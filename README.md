# Polygame

Very basic game engine on top of MonoGame.

## Supported Platforms

- iOS (via MonoGame.Framework.iOS)
- Android (via MonoGame.Framework.Android)
- Windows DX (via MonoGame.Framework.WindowsDX)
- Windows, macOS, Linux (via MonoGame.Framework.DesktopGL)

## Prerequisites

- .NET 10 SDK
- Android workload (`dotnet workload install android`)
- iOS workload (`dotnet workload install ios`) — macOS only

## Project Status

| Component                  | Build Status                                                                                                                                                                                    | Package                                                                                                                                                                                                                               |
|----------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Polygame.Core**          | [![build](https://github.com/anton-makarevich/SanetGameEngine/actions/workflows/core.yml/badge.svg)](https://github.com/anton-makarevich/SanetGameEngine/actions/workflows/core.yml)            | [![NuGet Version](https://img.shields.io/nuget/vpre/Polygame.Core?logo=nuget)](https://www.nuget.org/packages/Polygame.Core)                                                                                                          |
| **Polygame.Android**       | [![build](https://github.com/anton-makarevich/SanetGameEngine/actions/workflows/android.yml/badge.svg)](https://github.com/anton-makarevich/SanetGameEngine/actions/workflows/android.yml)      | [![NuGet Version](https://img.shields.io/nuget/vpre/Polygame.Android?logo=nuget)](https://www.nuget.org/packages/Polygame.Android)                                                                                                    |
| **Polygame.DesktopGL**     | [![build](https://github.com/anton-makarevich/SanetGameEngine/actions/workflows/desktopgl.yml/badge.svg)](https://github.com/anton-makarevich/SanetGameEngine/actions/workflows/desktopgl.yml)  | [![NuGet Version](https://img.shields.io/nuget/vpre/Polygame.DesktopGL?logo=nuget)](https://www.nuget.org/packages/Polygame.DesktopGL)                                                                                                |
| **Polygame.iOS**           | [![build](https://github.com/anton-makarevich/SanetGameEngine/actions/workflows/ios.yml/badge.svg)](https://github.com/anton-makarevich/SanetGameEngine/actions/workflows/ios.yml)              | [![NuGet Version](https://img.shields.io/nuget/vpre/Polygame.iOS?logo=nuget)](https://www.nuget.org/packages/Polygame.iOS)                                                                                                            |
| **Polygame.WindowsDX**     | [![build](https://github.com/anton-makarevich/SanetGameEngine/actions/workflows/windowsdx.yml/badge.svg)](https://github.com/anton-makarevich/SanetGameEngine/actions/workflows/windowsdx.yml)  | [![NuGet Version](https://img.shields.io/nuget/vpre/Polygame.WindowsDX?logo=nuget)](https://www.nuget.org/packages/Polygame.WindowsDX)                                                                                                |

## MonoGame Baseline

Version 3.8.4 with SDK-style projects and PackageReference.

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

## Historical Note

This project was originally developed in 2011-2012 for Windows Phone XNA.
Support for UWP, WP7, WP8, and 8.1 Universal has been removed.
