## Sonic Boom

Sonic Boom is a cross-platform, minimal **community-driven fork** of [NAudio](https://github.com/naudio/NAudio). The goal is to foster a community around the fork and maintain it together. The focus of the fork is audio playback, not audio mixing, conversion, visualization, or any other functionality.

That said, the fork currently supports re-encoding and audio format conversion; there are no plans to remove this functionality, either.

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

These are all original NAudio tutorials.

### Playback

* [Playing an Audio File from a WinForms application](Docs/PlayAudioFileWinForms.md)
* [Playing an Audio File from a Console application](Docs/PlayAudioFileConsoleApp.md)
* [Playing Audio from a URL](Docs/PlayAudioFromUrl.md)
* [Handling playback stopped](Docs/PlaybackStopped.md)
* [Understanding WaveStream, IWavePlayer and ISampleProvider](Docs/WaveProviders.md)

## Contributing

### Architectural Goals

The technical goals of Sonic Boom are:

- Maintain as minimal a version of NAudio as we can
- Keep up to date with NAudio, including new versions and PRs/fixes
- Try to keep the included NAudio as close to the upstream version as we can
- Make changes to the SonicBoom layer as much as possible

 e.g. if we want to implement looped playback, we'll implement it in `SonicBoom`, not `NAudio`.

## FAQ

### Q: Why does this exist?
As of 2025, there is no actively-maintained cross-platform audio library that you can use in .NET 8+ to play OGG files. I'm not an audio expert by any means; the .NET community needs to pick up where NAudio left off.

## Credits

Special thanks to Mark Heath, the original author and maintainer of [NAudio](https://github.com/naudio/NAudio), and all the contributers.
