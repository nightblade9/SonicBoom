# Sonic Boom

Sonic Boom is a minimal, open-source, cross-platform .NET audio library created to replace the no-longer-maintained NAudio, originally written by [Mark Heath](https://markheath.net).

## Features

* Play back audio using a variety of APIs
  * WaveOut
  * DirectSound
* Read audio from many standard file formats 
  * WAV
  * OGG
  * MP3 (using ACM, DMO or MFT)

## Getting Started

The easiest way to install Sonic Boom into your project is to clone this repository. In the future, you will be able to install the latest [NuGet package](https://www.nuget.org/packages/SonicBoom/). 

## Tutorials

### Playback

* [Playing an Audio File from a WinForms application](Docs/PlayAudioFileWinForms.md)
* [Playing an Audio File from a Console application](Docs/PlayAudioFileConsoleApp.md)
* [Playing Audio from a URL](Docs/PlayAudioFromUrl.md)
* [Handling playback stopped](Docs/PlaybackStopped.md)
* [Understanding WaveStream, IWavePlayer and ISampleProvider](Docs/WaveProviders.md)
uture support requests and bug reports. Are you willing to stick around on the forums and help out people using it?

## FAQ

### Why does this exist?

Because, as of 2025, there is no maintained cross-platform audio library that you can use in .NET 8+ to play OGG files.

# Credits

Special thanks to Mark Heath, the original author and maintainer of [NAudio](https://github.com/nightblade9/SonicBoom/).
