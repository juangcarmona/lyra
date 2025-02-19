using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lyra.Services;

namespace Lyra
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = Startup.ConfigureServices();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            PrintBanner(logger);

            var downloader = serviceProvider.GetRequiredService<DownloaderService>();

            if (args.Length == 0)
            {
                PrintUsage(logger);
                return;
            }

            if (args[0] == "--video" && args.Length > 1)
            {
                string videoUrl = args[1];
                await downloader.DownloadAudio(videoUrl);
            }
            else if (args[0] == "--playlist" && args.Length > 1)
            {
                string playlistUrl = args[1];
                await downloader.DownloadPlaylistAudios(playlistUrl);
            }
            else
            {
                logger.LogError("❌ Invalid command.");
                PrintUsage(logger);
            }
        }

        static void PrintBanner(ILogger logger)
        {
            logger.LogInformation("===============================================");
            logger.LogInformation("     🎵 LYRA - Lightweight Youtube Ripping Assistant 🎶");
            logger.LogInformation("===============================================");
        }

        static void PrintUsage(ILogger logger)
        {
            logger.LogWarning("Usage:");
            logger.LogWarning("  lyra --video <YouTube URL>      # Download a single video");
            logger.LogWarning("  lyra --playlist <Playlist URL>  # Download all audios from a playlist\n");
        }
    }
}
