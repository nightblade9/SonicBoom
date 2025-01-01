using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using NAudio.Wave;
using System.Diagnostics;
using System.Linq;

namespace NAudioTests.Aiff
{
    [TestFixture]
    public class AiffReaderTests
    {
        [Test]
        [Category("IntegrationTest")]
        public void ConvertAiffToWav()
        {
            // Arrange is done outside: popualting test data
            string testFolder = @"TestData";
            var aiffFiles = Directory.GetFiles(testFolder, "*.aiff");
            Assert.That(aiffFiles.Any());

            foreach (string file in aiffFiles)
            {
                string baseName=  Path.GetFileNameWithoutExtension(file);
                string wavFile = Path.Combine(testFolder, $"{baseName}.wav");
                string aiffFile = Path.Combine(file);
                Debug.WriteLine(String.Format("Converting {0} to wav", aiffFile));

                // Act/Assert: does not throw
                Assert.DoesNotThrow(() => ConvertAiffToWav(aiffFile, wavFile));
            }
        }

        private static void ConvertAiffToWav(string aiffFile, string wavFile)
        {
            using (var reader = new AiffFileReader(aiffFile))
            {
                using (var writer = new WaveFileWriter(wavFile, reader.WaveFormat))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    do
                    {
                        bytesRead = reader.Read(buffer, 0, buffer.Length);
                        writer.Write(buffer, 0, bytesRead);
                    } while (bytesRead > 0);
                }
            }
        }
    }
}
