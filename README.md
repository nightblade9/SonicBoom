## Sonic Boom

Sonic Boom is a minimal, **community-driven fork** of [NAudio](https://github.com/naudio/NAudio). The goal is for the community to handle ongoing extension and maintenance for this project, which is too much for any one person to handle.

This fork focuses mainly on playback. The extra project, `SonicBoom`, contains an `AudioPlayer` class that makes it easier to consume.

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

- Maintain as minimal a version of NAudio as we can get away with
- Keep up to date with NAudio, including new versions and PRs and fixes
- Try to keep the included NAudio as close to the upstream version as we can
- Make changes to the `SonicBoom` layer as much as possible
- Modernize, e.g. better automated testing

For example, if we want to implement looped playback, we'll implement it in `AudioPlayer`, not `NAudio`.

## FAQ

### Q: Why does this exist?
As of 2025, there is no actively-maintained cross-platform audio library that you can use in .NET 8+ to play OGG files. I'm not an audio expert by any means; the .NET community needs to pick up where NAudio left off.

## Credits

Special thanks to Mark Heath, the original author and maintainer of [NAudio](https://github.com/naudio/NAudio), and all the original and ongoing contributers.
