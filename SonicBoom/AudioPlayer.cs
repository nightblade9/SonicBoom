using System;
using System.IO;
using NAudio.Vorbis;
using NAudio.Wave;

namespace SonicBoom;

/// <summary>
/// A fire-and-forget audio player for audio.
/// Can loop OGG files.
/// </summary>
public class AudioPlayer : IDisposable
{
    /// <summary>
    /// An event that fires when playback automatically completes.
    /// Does not fire if you call Stop or Pause.
    /// If LoopPlayback is true, this fires on every loop.
    /// </summary>
    public event Action OnPlaybackComplete;

    /// <summary>
    /// Set to true to loop playback.
    /// </summary>
    public bool LoopPlayback { get; set; }
    
    private WaveOutEvent _waveOut;
    private WaveStream _reader;

    /// <summary>
    /// Create a new audio player, and load the audio file specified.
    /// </summary>
    public void Load(string fileName)
    {
        if (_waveOut != null)
        {
            _waveOut.Dispose();
        }

        if (_reader != null)
        {
            _reader.Dispose();
        }
        
        var fileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
        switch (fileExtension)
        {
            case "ogg":
                _reader = new VorbisWaveReader(fileName);
                break;
            case "wav":
                _reader = new WaveFileReader(fileName);
                break;
            default:
                throw new ArgumentException($"Not sure how to play {fileExtension} files");
        }

        _waveOut = new WaveOutEvent();
        _waveOut.Init(_reader);
        _waveOut.PlaybackStopped += (sender, stoppedArgs) => 
        {
            OnPlaybackComplete?.Invoke();
            if (!LoopPlayback)
            {
                return;
            }

            Play();
        };
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
    public void Play()
    {
        _reader.Seek(0, SeekOrigin.Begin);
        _waveOut.Play();
    }

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
            _reader?.Dispose();
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
