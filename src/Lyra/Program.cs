using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lyra.Services;
using System.Runtime.InteropServices;

namespace Lyra
{
    class Program
    {
        private static string _destinationPath = GetDefaultDownloadPath();

        static async Task Main(string[] args)
        {
            // Default values
            bool convertToMp3 = true;
            // string destinationPath = GetDefaultDownloadPath();

            // Parse arguments
            List<string> videoUrls = new();
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--convert-to-mp3":
                    case "-c":
                        if (i + 1 < args.Length && bool.TryParse(args[i + 1], out bool value))
                        {
                            convertToMp3 = value;
                            i++;
                        }
                        break;
                    case "--destination":
                    case "-d":
                        if (i + 1 < args.Length)
                        {
                            _destinationPath = args[i + 1];
                            i++;
                        }
                        break;
                    case "--video":
                    case "--playlist":
                        if (i + 1 < args.Length)
                        {
                            videoUrls.Add(args[i + 1]);
                            i++;
                        }
                        break;
                    default:
                        break;
                }
            }
            var serviceProvider = Startup.ConfigureServices(convertToMp3, _destinationPath);
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            PrintBanner(logger);

            if (videoUrls.Count == 0)
            {
                PrintUsage(logger);
                return;
            }

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
            logger.LogInformation("==============================================================");
            logger.LogInformation("     🎵 𝐋𝐘𝐑𝐀 - 𝐋ightweight 𝐘outube 𝐑ipping 𝐀ssistant 🎶      ");
            logger.LogInformation("==============================================================");
        }

        static void PrintUsage(ILogger logger)
        {
            logger.LogWarning("Usage:");
            logger.LogWarning("  lyra --video <YouTube URL>      # Download a single video");
            logger.LogWarning("  lyra --playlist <Playlist URL>  # Download all audios from a playlist");
            logger.LogWarning("  lyra -c <true|false>            # Convert to MP3 (default: true)");
            logger.LogWarning($"  lyra -d <path>                  # Set destination folder (default: {_destinationPath})");

            logger.LogInformation("Example usage:");
            logger.LogInformation("  lyra --video https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            logger.LogInformation("  lyra --playlist https://www.youtube.com/playlist?list=PL1234567890");
            logger.LogInformation("  lyra -c false --video https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            logger.LogInformation("  lyra -d ~/Music --video https://www.youtube.com/watch?v=dQw4w9WgXcQ");
        }

        static string GetDefaultDownloadPath()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            string basePath;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "LYRA", "downloads");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "LYRA", "downloads");
            }
            else
            {
                basePath = Path.Combine(Directory.GetCurrentDirectory(), "lyra_downloads");
            }

            string finalPath = Path.Combine(basePath, timestamp);
            return finalPath;
        }
    }
}
