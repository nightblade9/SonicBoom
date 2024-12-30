# 2.3.0

- Integrated NAudio/Vorbis into core NAudio project
- Removed several bits of functionality, including MIDI playback, UAP project, and demo projects

Merged in 24 NAudio PRs spanning from 2021-2024:

- [Retrieve error text for MmResult in MmException](https://github.com/naudio/NAudio/pull/1192)
- [ALSA (Linux audio support)](https://github.com/naudio/NAudio/pull/1182)
- [Added a more accurate comparison of floating point variables. Added memory release](https://github.com/naudio/NAudio/pull/1165)
- [Added Enumerator for AudioSessionControl in SessionCollection](https://github.com/naudio/NAudio/pull/1163)
- [1139 add in Net 6 to remove registry dependency](https://github.com/naudio/NAudio/pull/1143)
- [Switch to license expression ](https://github.com/naudio/NAudio/pull/1142/files)
- [Added events when fade-in/fade-out are complete](https://github.com/naudio/NAudio/pull/1136)
- [WaveOutEvent device capabilities](https://github.com/naudio/NAudio/pull/1125)
- [Fix a bug that WasapiCapture could not use exclusive mode](https://github.com/naudio/NAudio/pull/1122)
- [Don't write extraSize WAV-header field for PCM data](https://github.com/naudio/NAudio/pull/1098)
- [Discard extra data when it is too large to fit in the extraData buffer instead of throwing out of range exceptions](https://github.com/naudio/NAudio/pull/1044)
- [Fix floating point time conversion, move to integer with 3 digits in milliseconds](https://github.com/naudio/NAudio/pull/1032)
- [Added support for reading cue length](https://github.com/naudio/NAudio/pull/1013)
- [Add support for capture options in WASAPI loopback](https://github.com/naudio/NAudio/pull/1010)
- [WaveFormRendering.md Updated](https://github.com/naudio/NAudio/pull/1004)