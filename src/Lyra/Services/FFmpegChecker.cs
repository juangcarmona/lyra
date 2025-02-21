using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace Lyra.Utils
{
    public static class FFmpegChecker
    {
        public static void EnsureFFmpegIsAvailable(ILogger logger)
        {
            try
            {
                string ffmpegPath = FFmpeg.ExecutablesPath;
                string ffmpegExecutable = GetFFmpegExecutablePath();

                if (string.IsNullOrEmpty(ffmpegPath) || !File.Exists(ffmpegExecutable))
                {
                    throw new Exception("FFmpeg not found");
                }

                logger.LogInformation($"✅ FFmpeg is available at: {ffmpegPath}");
            }
            catch (Exception)
            {
                logger.LogWarning("⚠️ FFmpeg not found. Attempting to download it automatically...");

                if (TryDownloadFFmpeg(logger))
                {
                    logger.LogInformation("✅ FFmpeg successfully downloaded and set up.");
                }
                else
                {
                    logger.LogError("❌ FFmpeg could not be downloaded. Please install it manually.");
                    ProvideInstallationInstructions(logger);
                    // throw new Exception("FFmpeg installation failed.");
                }
            }
        }

        private static bool TryDownloadFFmpeg(ILogger logger)
        {
            try
            {
                string downloadPath = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg");
                FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official).Wait();
                FFmpeg.SetExecutablesPath(downloadPath);

                string ffmpegExecutable = GetFFmpegExecutablePath();
                return File.Exists(ffmpegExecutable);
            }
            catch (Exception ex)
            {
                logger.LogError($"❌ FFmpeg download failed: {ex.Message}");
                return false;
            }
        }

        private static void ProvideInstallationInstructions(ILogger logger)
        {
            logger.LogWarning("\n🔹 Manual Installation Instructions:");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.LogWarning("📌 Windows: Download FFmpeg from https://ffmpeg.org/download.html");
                logger.LogWarning("📌 Extract and add the 'bin' folder to your system PATH.");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                logger.LogWarning("🍏 macOS: Install FFmpeg using Homebrew:");
                logger.LogWarning("    brew install ffmpeg");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                logger.LogWarning("🐧 Linux: Install FFmpeg using your package manager:");
                logger.LogWarning("    sudo apt install ffmpeg  (Debian/Ubuntu)");
                logger.LogWarning("    sudo dnf install ffmpeg  (Fedora)");
                logger.LogWarning("    sudo pacman -S ffmpeg  (Arch)");
            }
            else
            {
                logger.LogWarning("⚠️ Unknown OS detected. Please install FFmpeg manually from https://ffmpeg.org/download.html");
            }
        }

        private static string GetFFmpegExecutablePath()
        {
            string ffmpegDir = FFmpeg.ExecutablesPath;
            string ffmpegExecutable = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffmpeg.exe" : "ffmpeg";
            return Path.Combine(ffmpegDir, ffmpegExecutable);
        }
    }
}
