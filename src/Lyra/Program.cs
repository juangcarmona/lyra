using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lyra.Services;
using Lyra.Utils;
using Microsoft.Extensions.Logging.Console;
using Lyra.Logging;

namespace Lyra
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Load configuration from appsettings.json
            var basePath = AppContext.BaseDirectory; // Points to bin/Debug/net8.0/

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)  
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();


            // Set up Dependency Injection with Logging
            var serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders(); // Remove default providers
                    loggingBuilder.AddConsoleFormatter<SimpleConsoleFormatter, ConsoleFormatterOptions>();
                    loggingBuilder.AddConsole(options =>
                    {
                        options.FormatterName = "simple"; // Use custom formatter
                    });
                    loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                })
                .AddSingleton<DownloaderService>()
                .BuildServiceProvider();


            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            PrintBanner(logger);

            try
            {
                FFmpegChecker.EnsureFFmpegIsAvailable(logger);
            }
            catch (Exception)
            {
                logger.LogError("❌ FFmpeg is required for this application to run.");
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
                await downloader.DownloadVideo(videoUrl);
            }
            else if (args[0] == "--playlist" && args.Length > 1)
            {
                string playlistUrl = args[1];
                await downloader.DownloadPlaylist(playlistUrl);
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
