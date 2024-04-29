﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NAudio.Sdl2.Interop;
using NAudio.Sdl2.Structures;
using NAudio.Wave;
using static NAudio.Sdl2.Interop.SDL;

// ReSharper disable once CheckNamespace
namespace NAudio.Sdl2
{
    /// <summary>
    /// WaveOut provider via SDL2 backend
    /// </summary>
    public class WaveOutSdl : IWavePlayer
    {
        private readonly SynchronizationContext syncContext;
        private IWaveProvider waveStream;
        private uint deviceNumber;
        private volatile PlaybackState playbackState;
        private AutoResetEvent callbackEvent;
        private double adjustLatencyPercent;
        private ushort frameSize;
        private byte[] frameBuffer;

        /// <summary>
        /// Indicates playback has stopped automatically
        /// </summary>
        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        /// <summary>
        /// Prepares a wave output device for recording
        /// </summary>
        public WaveOutSdl()
        {
            syncContext = SynchronizationContext.Current;
            DeviceId = -1;
            AudioConversion = AudioConversion.None;
            DesiredLatency = 300;
            AdjustLatencyPercent = 0.1;
        }

        /// <summary>
        /// Returns the number of WaveOutSdl devices available in the system
        /// </summary>
        public static int DeviceCount => SdlBindingWrapper.GetPlaybackDevicesNumber();

        /// <summary>
        /// Retrieves the capabilities of a WaveOutSdl device
        /// </summary>
        /// <param name="deviceId">Device to test</param>
        /// <returns>The WaveOutSdl device capabilities</returns>
        public static WaveOutSdlCapabilities GetCapabilities(int deviceId)
        {
            var deviceName = SdlBindingWrapper.GetPlaybackDeviceName(deviceId);
            var deviceAudioSpec = SdlBindingWrapper.GetPlaybackDeviceAudioSpec(deviceId);
            var deviceBitSize = SdlBindingWrapper.GetAudioFormatBitSize(deviceAudioSpec.format);
            return new WaveOutSdlCapabilities
            {
                DeviceNumber = deviceId,
                DeviceName = deviceName,
                Bits = deviceBitSize,
                Channels = deviceAudioSpec.channels,
                Format = deviceAudioSpec.format,
                Frequency = deviceAudioSpec.freq,
                Samples = deviceAudioSpec.samples,
                Silence = deviceAudioSpec.silence,
                Size = deviceAudioSpec.size
            };
        }

        /// <summary>
        /// Retrieves the capabilities list of a WaveOutSdl devices
        /// </summary>
        /// <returns>The WaveOutSdl capabilities list</returns>
        public static List<WaveOutSdlCapabilities> GetCapabilitiesList()
        {
            List<WaveOutSdlCapabilities> list = new List<WaveOutSdlCapabilities>();
            var deviceCount = WaveInSdl.DeviceCount;
            for (int index = 0; index < deviceCount; index++)
            {
                list.Add(GetCapabilities(index));
            }
            return list;
        }

        /// <summary>
        /// Retrieves the capabilities of a WaveOutSdl default device
        /// <para>This function is available since SDL 2.24.0</para>
        /// </summary>
        /// <returns>The WaveOutSdl default device capabilities</returns>
        public static WaveOutSdlCapabilities GetDefaultDeviceCapabilities()
        {
            SdlBindingWrapper.GetPlaybackDeviceDefaultAudioInfo(out var deviceName, out var deviceAudioSpec);
            var deviceBitSize = SdlBindingWrapper.GetAudioFormatBitSize(deviceAudioSpec.format);
            return new WaveOutSdlCapabilities
            {
                DeviceNumber = -1,
                DeviceName = deviceName,
                Bits = deviceBitSize,
                Channels = deviceAudioSpec.channels,
                Format = deviceAudioSpec.format,
                Frequency = deviceAudioSpec.freq,
                Samples = deviceAudioSpec.samples,
                Silence = deviceAudioSpec.silence,
                Size = deviceAudioSpec.size
            };
        }

        /// <summary>
        /// Gets or sets the device id
        /// Should be set before a call to <see cref="Init(IWaveProvider)"/>
        /// <para>This must be between -1 and <see cref="DeviceCount"/> - 1</para>
        /// <para>-1 means stick to default device</para>
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the desired latency in milliseconds
        /// <para>Should be set before a call to <see cref="Init(IWaveProvider)"/></para>
        /// </summary>
        public int DesiredLatency { get; set; }

        /// <summary>
        /// Gets or sets the desired latency adjustment in percent
        /// <para>Value must be between 0 and 1</para>
        /// <para>This percent only affects the playback wait</para>
        /// </summary>
        public double AdjustLatencyPercent
        {
            get => adjustLatencyPercent;
            set => adjustLatencyPercent = value >= 0 && value <= 1
                ? value
                : throw new Exception("The percent value must be between 0 and 1");
        }

        /// <summary>
        /// Volume for this device 1.0 is full scale
        /// </summary>
        public float Volume
        {
            get => throw new SdlException("Volume mixer is not implemented");
            set => throw new SdlException("Volume mixer is not implemented");
        }

        /// <summary>
        /// Playback State
        /// </summary>
        public PlaybackState PlaybackState => playbackState;

        /// <summary>
        /// Gets playback state directly from sdl
        /// </summary>
        public PlaybackState SdlState
        {
            get
            {
                var status = SdlBindingWrapper.GetDeviceStatus(deviceNumber);
                switch (status)
                {
                    case SDL_AudioStatus.SDL_AUDIO_PLAYING:
                        return PlaybackState.Playing;
                    case SDL_AudioStatus.SDL_AUDIO_PAUSED:
                        return PlaybackState.Paused;
                    default:
                        return PlaybackState.Stopped;
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="Wave.WaveFormat"/> instance indicating the format the hardware is using.
        /// </summary>
        public WaveFormat OutputWaveFormat => waveStream.WaveFormat;

        /// <summary>
        /// Gets a <see cref="Wave.WaveFormat"/> instance indicating what the format is actually using.
        /// <para>This property accessible after <see cref="Init(IWaveProvider)"/> call</para>
        /// </summary>
        public WaveFormat ActualOutputWaveFormat { get; private set; }

        /// <summary>
        /// Audio conversion features
        /// </summary>
        public AudioConversion AudioConversion { get; set; }

        /// <summary>
        /// Initializes the WaveOut device
        /// </summary>
        /// <param name="waveProvider">WaveProvider to play</param>
        public void Init(IWaveProvider waveProvider)
        {
            if (playbackState != PlaybackState.Stopped)
            {
                throw new InvalidOperationException("Can't re-initialize during playback");
            }
            callbackEvent = new AutoResetEvent(false);
            waveStream = waveProvider;
            frameSize = (ushort)waveProvider.WaveFormat.ConvertLatencyToByteSize(DesiredLatency);
            frameBuffer = new byte[frameSize];
            var desiredSpec = new SDL_AudioSpec();
            desiredSpec.freq = waveProvider.WaveFormat.SampleRate;
            desiredSpec.format = GetAudioDataFormat();
            desiredSpec.channels = (byte)waveProvider.WaveFormat.Channels;
            desiredSpec.silence = 0;
            desiredSpec.samples = frameSize;
            var deviceName = SdlBindingWrapper.GetPlaybackDeviceName(DeviceId);
            var openDeviceNumber = SdlBindingWrapper.OpenPlaybackDevice(deviceName, ref desiredSpec, out var obtainedSpec, AudioConversion);
            var bitSize = SdlBindingWrapper.GetAudioFormatBitSize(obtainedSpec.format);
            ActualOutputWaveFormat = new WaveFormat(obtainedSpec.freq, bitSize, obtainedSpec.channels);
            deviceNumber = openDeviceNumber;
        }

        /// <summary>
        /// Start playing the audio from the WaveStream
        /// </summary>
        public void Play()
        {
            if (waveStream == null)
            {
                throw new InvalidOperationException("Must call Init first");
            }
            if (playbackState == PlaybackState.Stopped)
            {
                Resume();
                ThreadPool.QueueUserWorkItem(state => PlaybackThread(), null);
            }
            else if (playbackState == PlaybackState.Paused)
            {
                Resume();
            }
        }

        /// <summary>
        /// Stop the audio
        /// </summary>
        public void Stop()
        {
            if (playbackState != PlaybackState.Stopped)
            {
                playbackState = PlaybackState.Stopped;
                SdlBindingWrapper.StopPlaybackDevice(deviceNumber);
                SdlBindingWrapper.ClosePlaybackDevice(deviceNumber);
            }
        }

        /// <summary>
        /// Pause the audio
        /// </summary>
        public void Pause()
        {
            if (playbackState == PlaybackState.Playing)
            {
                playbackState = PlaybackState.Paused;
                SdlBindingWrapper.StopPlaybackDevice(deviceNumber);
            }
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose pattern
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
        }

        /// <summary>
        /// Resume playing
        /// </summary>
        private void Resume()
        {
            SdlBindingWrapper.StartPlaybackDevice(deviceNumber);
            playbackState = PlaybackState.Playing;
            callbackEvent.Set();
        }

        /// <summary>
        /// Returns the audio format guessed by <see cref="WaveFormat.BitsPerSample"/>
        /// </summary>
        /// <returns>Audio format</returns>
        private ushort GetAudioDataFormat()
        {
            switch (OutputWaveFormat.BitsPerSample)
            {
                case 8:
                    return AUDIO_S8;
                case 16:
                    return AUDIO_S16SYS;
                case 32:
                    return AUDIO_S32SYS;
                default:
                    return (ushort)OutputWaveFormat.BitsPerSample;
            }
        }

        /// <summary>
        /// Thread at which playback is happen
        /// </summary>
        private void PlaybackThread()
        {
            Exception exception = null;
            try
            {
                DoPlayback();
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                playbackState = PlaybackState.Stopped;
                // we're exiting our background thread
                RaisePlaybackStoppedEvent(exception);
            }
        }

        /// <summary>
        /// Playback process
        /// </summary>
        private unsafe void DoPlayback()
        {
            while (playbackState != PlaybackState.Stopped)
            {
                // workaround to get rid of stuttering
                // i assume on different hardware adjusting must be different
                // is it possible that callbacks will help?
                var adjustedLatency = DesiredLatency - (int)(DesiredLatency * AdjustLatencyPercent);
                if (!callbackEvent.WaitOne(adjustedLatency))
                {
                    if (playbackState == PlaybackState.Playing)
                    {
                        Debug.WriteLine("WARNING: WaveOutSdl callback event timeout");
                    }
                }

                if (playbackState == PlaybackState.Playing)
                {
                    Array.Clear(frameBuffer, 0, frameBuffer.Length);
                    var readSize = waveStream.Read(frameBuffer, 0, frameBuffer.Length);

                    if (readSize == 0)
                    {
                        playbackState = PlaybackState.Stopped;
                        callbackEvent.Set();
                    }

                    fixed (byte* ptr = &frameBuffer[0])
                    {
                        SdlBindingWrapper.QueueAudio(deviceNumber, (IntPtr)ptr, (uint)readSize);
                    }
                }
            }
        }

        /// <summary>
        /// Raise playback stopped event
        /// </summary>
        /// <param name="e"></param>
        private void RaisePlaybackStoppedEvent(Exception e)
        {
            var handler = PlaybackStopped;
            if (handler != null)
            {
                if (syncContext == null)
                {
                    handler(this, new StoppedEventArgs(e));
                }
                else
                {
                    syncContext.Post(state => handler(this, new StoppedEventArgs(e)), null);
                }
            }
        }
    }
}
