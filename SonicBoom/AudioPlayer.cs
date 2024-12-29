using System;

namespace SonicBoom;

public class AudioPlayer : IDisposable
{
    private WaveOutEvent _waveOutEvent;

    public AudioPlayer(string fileName)
    {
        var reader = new VorbisWaveReader($fileName);
        _waveOut = new WaveOutEvent();
        _waveOut.Init(reader);
    }


    public void Stop() => _waveOut?.Pause(); // WaveOutEvent.Stop doesn't work ...

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _waveOut?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
