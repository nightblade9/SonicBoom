## Sonic Boom

Sonic Boom is a cross-platform, minimal, open-source .NET audio library created to replace the no-longer-maintained [NAudio](https://github.com/naudio/NAudio).

## Features

* Play back audio using a variety of APIs
  * WaveOut
  * DirectSound
* Read audio from many standard file formats 
  * WAV
  * OGG
  * MP3 (using ACM, DMO or MFT)
* Loop OGG files

## Getting Started

The easiest way to install Sonic Boom into your project is to install the latest [NuGet package](https://www.nuget.org/packages/SonicBoom/). 

## Tutorials

### Playback

* [Playing an Audio File from a WinForms application](Docs/PlayAudioFileWinForms.md)
* [Playing an Audio File from a Console application](Docs/PlayAudioFileConsoleApp.md)
* [Playing Audio from a URL](Docs/PlayAudioFromUrl.md)
* [Handling playback stopped](Docs/PlaybackStopped.md)
* [Understanding WaveStream, IWavePlayer and ISampleProvider](Docs/WaveProviders.md)

## FAQ

**Q: Why does this exist?**
**A:** Because, as of 2025, there is no maintained cross-platform audio library that you can use in .NET 8+ to play OGG files.

## Credits

Special thanks to Mark Heath, the original author and maintainer of [NAudio](https://github.com/naudio/NAudio), and all the contributers.
