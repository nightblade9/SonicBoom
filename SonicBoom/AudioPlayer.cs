using System;
using NAudio.Vorbis;
using NAudio.Wave;

namespace SonicBoom;

/// <summary>
/// A fire-and-forget audio player for audio.
/// Can loop OGG files.
/// </summary>
public class AudioPlayer : IDisposable
{
    private WaveOutEvent _waveOut;

    /// <summary>
    /// Create a new audio player, and load the audio file specified.
    /// </summary>
    public AudioPlayer(string fileName)
    {
        var reader = new VorbisWaveReader(fileName);
        _waveOut = new WaveOutEvent();
        _waveOut.Init(reader);
    }

    /// <summary>
    /// True if playback should immediately loop once complete.
    /// </summary>
    public bool LoopPlayback
    {
        get { return _waveOut.LoopPlayback; }
        set { _waveOut.LoopPlayback = value; }
    }

    /// <summary>
    /// Volume to play back at; 0.0 is silent and 1.0 is maximum volume.
    /// </summary>
    public float Volume
    {
        get { return _waveOut.Volume; }
        set { _waveOut.Volume = value; }
    }

    /// <summary>
    /// Plays the audio file.
    /// </summary>
    public void Play() => _waveOut.Play();

    /// <summary>
    /// Stops audio playback.
    /// </summary>
    public void Stop() => _waveOut.Pause(); // WaveOutEvent.Stop doesn't work ...

    /// <summary>
    /// Disposes necessary resources.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _waveOut?.Dispose();
        }
    }

    /// <summary>
    /// Disposes necessary resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
