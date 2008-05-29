﻿using System;
using System.Collections.Generic;
using System.Text;
using NAudio.CoreAudioApi.Interfaces;
using System.Runtime.InteropServices;
using NAudio.Wave;

namespace NAudio.CoreAudioApi
{
    /// <summary>
    /// Windows Vista CoreAudio AudioClient
    /// </summary>
    public class AudioClient
    {
        IAudioClient audioClientInterface;
        
        internal AudioClient(IAudioClient audioClientInterface)
        {
            this.audioClientInterface = audioClientInterface;
        }

        /// <summary>
        /// Mix Format,
        /// Can be called before initialize
        /// </summary>
        public WaveFormatExtensible MixFormat
        {
            get
            {
                IntPtr waveFormatPointer;
                Marshal.ThrowExceptionForHR(audioClientInterface.GetMixFormat(out waveFormatPointer));
                WaveFormatExtensible waveFormat = new WaveFormatExtensible(44100,32,2);
                Marshal.PtrToStructure(waveFormatPointer, waveFormat);
                Marshal.FreeCoTaskMem(waveFormatPointer);
                return waveFormat;
            }
        }

        /// <summary>
        /// Initialize the Audio Client
        /// </summary>
        /// <param name="shareMode">Share Mode</param>
        /// <param name="streamFlags">Stream Flags</param>
        /// <param name="bufferDuration">Buffer Duration</param>
        /// <param name="periodicity">Periodicity</param>
        /// <param name="waveFormat">Wave Format</param>
        /// <param name="audioSessionGuid">Audio Session GUID (can be null)</param>
        public void Initialize(AudioClientShareMode shareMode,
            AudioClientStreamFlags streamFlags,
            long bufferDuration,
            long periodicity,
            WaveFormat waveFormat,
            Guid audioSessionGuid)
        {
            Marshal.ThrowExceptionForHR(audioClientInterface.Initialize(shareMode,streamFlags,bufferDuration,periodicity,waveFormat, ref audioSessionGuid));
        }

        /// <summary>
        /// Gets the buffer size (must initialize first)
        /// </summary>
        public int BufferSize
        {
            get
            {
                uint bufferSize;                
                Marshal.ThrowExceptionForHR(audioClientInterface.GetBufferSize(out bufferSize));
                return (int) bufferSize;
            }
        }

        /// <summary>
        /// Gets the current padding (must initialize first)
        /// </summary>
        public int CurrentPadding
        {
            get
            {
                int currentPadding;
                Marshal.ThrowExceptionForHR(audioClientInterface.GetCurrentPadding(out currentPadding));
                return currentPadding;
            }
        }

        /// <summary>
        /// Gets the default device period (can be called before initialize)
        /// </summary>
        public long DefaultDevicePeriod
        {
            get
            {
                long defaultDevicePeriod;
                long minimumDevicePeriod;
                Marshal.ThrowExceptionForHR(audioClientInterface.GetDevicePeriod(out defaultDevicePeriod, out minimumDevicePeriod));
                return defaultDevicePeriod;
            }
        }

        /// <summary>
        /// Gets the minimum device period (can be called before initialize)
        /// </summary>
        public long MinimumDevicePeriod
        {
            get
            {
                long defaultDevicePeriod;
                long minimumDevicePeriod;
                Marshal.ThrowExceptionForHR(audioClientInterface.GetDevicePeriod(out defaultDevicePeriod, out minimumDevicePeriod));
                return minimumDevicePeriod;
            }
        }

        // TODO: GetService:
        // IID_IAudioCaptureClient
        // IID_IAudioClock
        // IID_IAudioRenderClient
        // IID_IAudioSessionControl
        // IID_IAudioStreamVolume
        // IID_IChannelAudioVolume
        // IID_ISimpleAudioVolume

        /// <summary>
        /// Gets the AudioRenderClient service
        /// </summary>
        public AudioRenderClient AudioRenderClient
        {
            get
            {
                object audioRenderClientInterface;
                Guid audioRenderClientGuid = new Guid("F294ACFC-3146-4483-A7BF-ADDCA7C260E2");
                Marshal.ThrowExceptionForHR(audioClientInterface.GetService(ref audioRenderClientGuid, out audioRenderClientInterface));
                return new AudioRenderClient((IAudioRenderClient)audioRenderClientInterface);
            }
        }

        /// <summary>
        /// Determines if the specified mode is supported
        /// </summary>
        /// <param name="shareMode">Share Mode</param>
        /// <param name="desiredFormat">Desired Format</param>
        /// <returns></returns>
        public bool IsFormatSupported(AudioClientShareMode shareMode,
            WaveFormat desiredFormat)
        {
            IntPtr closestMatchPointer = IntPtr.Zero;
            int hresult;
            try
            {
                hresult = audioClientInterface.IsFormatSupported(shareMode, desiredFormat, out closestMatchPointer);
                // S_OK is 0, S_FALSE = 1
                if (hresult == 0)
                {
                    // directly supported
                    return true;
                }
                if (hresult == 1)
                {
                    // a closest match should be supplied
                    if (closestMatchPointer == IntPtr.Zero)
                    {
                        // shouldn't happen
                        throw new NotSupportedException("Not supported but no closest match");
                    }
                    // This is how to get the closest match...
                    //WaveFormatExtensible closestMatchFormat = new WaveFormatExtensible(44100, 32, 2);
                    //Marshal.PtrToStructure(closestMatchPointer, closestMatchFormat);
                    Marshal.FreeCoTaskMem(closestMatchPointer);
                    //return closestMatchFormat;
                    return false;
                }
            }
            catch (COMException ce)
            {
                if (ce.ErrorCode == (int)AudioClientErrors.UnsupportedFormat)
                {
                    return false;
                }
                throw;
            }
            throw new NotSupportedException("Unknown hresult " + hresult.ToString());
        }

        /// <summary>
        /// Starts the audio stream
        /// </summary>
        public void Start()
        {
            audioClientInterface.Start();
        }

        /// <summary>
        /// Stops the audio stream.
        /// </summary>
        public void Stop()
        {
            audioClientInterface.Stop();
        }

        /// <summary>
        /// Resets the audio stream
        /// Reset is a control method that the client calls to reset a stopped audio stream. 
        /// Resetting the stream flushes all pending data and resets the audio clock stream 
        /// position to 0. This method fails if it is called on a stream that is not stopped
        /// </summary>
        public void Reset()
        {
            audioClientInterface.Reset();
        }

        // TODO:
        // int GetStreamLatency(out long streamLatency);        
        // int SetEventHandle(IntPtr eventHandle);

    }
}
