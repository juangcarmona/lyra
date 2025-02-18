using System;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Lyra.Services
{
    public class ConversionService
    {
        public async Task<string> ConvertToMp3(string videoPath)
        {
            string mp3Path = Path.ChangeExtension(videoPath, ".mp3");

            try
            {
                Console.WriteLine("🎵 Converting to mp3...");

                var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(videoPath, mp3Path);

                conversion.OnProgress += (sender, args) =>
                {
                    Console.Write($"\rProgreso: {args.Percent}%");
                };

                await conversion.Start();
                Console.WriteLine($"\n🎶 MP3 saved: {mp3Path}");

                return mp3Path;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during conversion: {ex.Message}");

                if (File.Exists(mp3Path))
                {
                    File.Delete(mp3Path);
                    Console.WriteLine($"🗑️ MP3 deleted: {mp3Path}");
                }

                throw;
            }
        }
    }
}
