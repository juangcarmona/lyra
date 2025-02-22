using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using System.Diagnostics;

public class ConversionService
{
    private readonly ILogger<ConversionService> _logger;

    public ConversionService(ILogger<ConversionService> logger)
    {
        _logger = logger;
        EnsureFFmpegIsAvailable();
    }


    public async Task<string> ConvertToMp3(string inputPath)
    {
        string outputPath = Path.ChangeExtension(inputPath, ".mp3");

        try
        {
            _logger.LogInformation($"🎵 Converting {inputPath} to MP3...");

            var conversion = await FFmpeg.Conversions.New()
                .AddParameter($"-i \"{inputPath}\" -q:a 2 \"{outputPath}\"") // VBR MP3
                .Start();

            _logger.LogInformation($"✅ Conversion complete: {outputPath}");
            return outputPath;
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error converting to MP3: {ex.Message}");
            throw;
        }
    }

    static string GetFFmpegPath()
    {
        string basePath;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LYRA", "ffmpeg");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".lyra", "ffmpeg");
        }
        else
        {
            basePath = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg");
        }

        Directory.CreateDirectory(basePath);

        return basePath;
    }

    private void EnsureFFmpegIsAvailable()
    {
        string ffmpegPath = GetFFmpegPath();
        string ffmpegExecutable = Path.Combine(ffmpegPath, GetFFmpegExecutable());
        string ffprobeExecutable = Path.Combine(ffmpegPath, GetFFprobeExecutable());

        try
        {
            if (IsFFmpegInstalled())
            {
                _logger.LogInformation("✅ FFmpeg is available in system.");
                return;
            }

            if (File.Exists(ffmpegExecutable) && File.Exists(ffprobeExecutable))
            {
                FFmpeg.SetExecutablesPath(ffmpegPath);
                _logger.LogInformation($"✅ FFmpeg is available at: {ffmpegPath}");
                return;
            }

            throw new FileNotFoundException("FFmpeg not found");
        }
        catch (Exception)
        {
            _logger.LogWarning("⚠️ FFmpeg not found. Downloading automatically...");

            if (DownloadAndSetupFFmpeg(ffmpegPath))
            {
                _logger.LogInformation("✅ FFmpeg successfully downloaded and set up.");
            }
            else
            {
                _logger.LogCritical("❌ FFmpeg download failed. The program **CANNOT** continue.");
                throw new Exception("FFmpeg installation failed.");
            }
        }
    }

    private bool DownloadAndSetupFFmpeg(string ffmpegPath)
    {
        try
        {
            _logger.LogInformation($"📥 Downloading FFmpeg to: {ffmpegPath}...");

            FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ffmpegPath).Wait();

            // Verificar si los archivos existen
            string ffmpegExecutable = Path.Combine(ffmpegPath, GetFFmpegExecutable());
            string ffprobeExecutable = Path.Combine(ffmpegPath, GetFFprobeExecutable());

            if (!File.Exists(ffmpegExecutable) || !File.Exists(ffprobeExecutable))
            {
                throw new Exception("FFmpeg download completed but executables were not found.");
            }

            FFmpeg.SetExecutablesPath(ffmpegPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ FFmpeg download failed: {ex.Message}");
            return false;
        }
    }

    private bool IsFFmpegInstalled()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = "-version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static string GetFFmpegExecutable()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffmpeg.exe" : "ffmpeg";
    }

    private static string GetFFprobeExecutable()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffprobe.exe" : "ffprobe";
    }
}