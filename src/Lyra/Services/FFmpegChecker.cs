using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Xabe.FFmpeg;

namespace Lyra.Utils
{
    public static class FFmpegChecker
    {
        public static void EnsureFFmpegIsAvailable(ILogger logger)
        {
            try
            {
                // Check if FFmpeg path is set
                string ffmpegPath = FFmpeg.ExecutablesPath;

                if (string.IsNullOrEmpty(ffmpegPath) || !File.Exists(Path.Combine(ffmpegPath, GetFFmpegExecutable())))
                {
                    throw new Exception("FFmpeg not found");
                }

                logger.LogInformation("✅ FFmpeg is available at: " + ffmpegPath);
            }
            catch (Exception)
            {
                logger.LogError("❌ FFmpeg is not installed or not found in the system PATH.");
                ProvideInstallationInstructions(logger);
                throw;
            }
        }

        private static void ProvideInstallationInstructions(ILogger logger)
        {
            logger.LogWarning("\n🔹 Installation Instructions:");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.LogWarning("  📌 Windows: Download FFmpeg from https://ffmpeg.org/download.html");
                logger.LogWarning("  📌 Add the 'bin' folder to your system PATH.");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                logger.LogWarning("  🍏 macOS: Install FFmpeg using Homebrew:");
                logger.LogWarning("    brew install ffmpeg");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                logger.LogWarning("  🐧 Linux: Install FFmpeg using your package manager:");
                logger.LogWarning("    sudo apt install ffmpeg  (Debian/Ubuntu)");
                logger.LogWarning("    sudo dnf install ffmpeg  (Fedora)");
                logger.LogWarning("    sudo pacman -S ffmpeg  (Arch)");
            }
            else
            {
                logger.LogWarning("  ⚠️ Unknown OS detected. Please install FFmpeg manually from https://ffmpeg.org/download.html");
            }
        }

        private static string GetFFmpegExecutable()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffmpeg.exe" : "ffmpeg";
        }
    }
}
