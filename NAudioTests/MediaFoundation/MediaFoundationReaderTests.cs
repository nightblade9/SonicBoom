using System;
using System.Diagnostics;
using System.IO;
using NAudio.MediaFoundation;
using NAudio.Wave;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NAudioTests.MediaFoundation
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class MediaFoundationReaderTests
    {
        [Test]
        public void CanReadAnAac()
        {
            // Arrange is done outside this test
            var testFile = Path.Combine("TestData", "taskCompleted.aac");
            var reader = new MediaFoundationReader(testFile);
            Console.WriteLine(reader.WaveFormat);
            
            var buffer = new byte[reader.WaveFormat.AverageBytesPerSecond];
            int bytesRead;
            long total = 0;
            
            // Act
            while((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                total += bytesRead;
            }
            
            // Assert
            Assert.IsTrue(total > 0);
        }
    }

    [TestFixture]
    [Category("IntegrationTest")]
    public class MediaFoundationEncoderTests
    {
        [Test]
        public void CanEncodeLargeGSM610FileToMp3()
        {
            // Arrange is done outside of the test
            string fileInPath = Path.Combine("TestData", "taskCompleted.wav");
            string fileOutPath = Path.Combine("TestData", "taskCompleted.mp3");

            Stopwatch sw = Stopwatch.StartNew();
            using (var wavToConvert = new WaveFileReader(fileInPath))
            using (var converter = WaveFormatConversionStream.CreatePcmStream(wavToConvert))
            {
                Console.WriteLine($"Format in = {wavToConvert.WaveFormat}, Sample rate {wavToConvert.WaveFormat.SampleRate}");
                Console.WriteLine($"Format out = {converter.WaveFormat}, Sample rate {converter.WaveFormat.SampleRate}");

                var mediaType = MediaFoundationEncoder.SelectMediaType(AudioSubtypes.MFAudioFormat_MP3, converter.WaveFormat, 0);
                if (mediaType == null) 
                {
                    throw new InvalidOperationException("No suitable MP3 encoders available");
                }

                Console.WriteLine($"MediaType = {(mediaType.AverageBytesPerSecond * 8)/1000}kbps, Sample rate {mediaType.SampleRate}, Channels: {mediaType.ChannelCount}");
                
                using (var encoder = new MediaFoundationEncoder(mediaType))
                {
                    // do a whole minute at a time - makes it faster on long files
                    // n.b. tried 10 minutes, didn't result in any noticable improvement
                    // limitation is now mostly the ACM GSM610 decoder

                    // Act/Assert
                    encoder.DefaultReadBufferSize = converter.WaveFormat.AverageBytesPerSecond * 60; 
                    Assert.DoesNotThrow(() => encoder.Encode(fileOutPath, converter));
                }
            }
            Console.WriteLine($"Converted in {sw.Elapsed}");
        }
    }
}
