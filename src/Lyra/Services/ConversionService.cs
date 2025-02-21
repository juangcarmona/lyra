using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

public class ConversionService
{
    private readonly ILogger<ConversionService> _logger;
    private readonly string _ffmpegPath;

    public ConversionService(ILogger<ConversionService> logger)
    {
        _logger = logger;
        _ffmpegPath = Path.Combine(AppContext.BaseDirectory, "ffmpeg");
        EnsureFFmpegIsAvailable();
    }

    private void EnsureFFmpegIsAvailable()
    {
        string ffmpegExecutable = Path.Combine(_ffmpegPath, GetFFmpegExecutable());
        string ffprobeExecutable = Path.Combine(_ffmpegPath, GetFFprobeExecutable());

        try
        {
            if (File.Exists(ffmpegExecutable) && File.Exists(ffprobeExecutable))
            {
                FFmpeg.SetExecutablesPath(_ffmpegPath);
                _logger.LogInformation($"✅ FFmpeg is available at: {_ffmpegPath}");
                return;
            }

            throw new FileNotFoundException("FFmpeg not found");
        }
        catch (Exception)
        {
            _logger.LogWarning("⚠️ FFmpeg not found. Downloading automatically...");

            if (DownloadAndSetupFFmpeg())
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

    private bool DownloadAndSetupFFmpeg()
    {
        try
        {
            string executionPath = Path.Combine(Environment.CurrentDirectory);

            _logger.LogInformation($"📥 Downloading FFmpeg to: {executionPath}...");

            FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official).Wait();

            // Locate the downloaded executables correctly
            string tempFfmpeg = LocateDownloadedFile(executionPath, GetFFmpegExecutable());
            string tempFfprobe = LocateDownloadedFile(executionPath, GetFFprobeExecutable());

            if (string.IsNullOrEmpty(tempFfmpeg) || string.IsNullOrEmpty(tempFfprobe))
                throw new Exception("FFmpeg download completed but executables were not found.");

            // Move to the correct location
            Directory.CreateDirectory(_ffmpegPath);
            File.Move(tempFfmpeg, Path.Combine(_ffmpegPath, GetFFmpegExecutable()), true);
            File.Move(tempFfprobe, Path.Combine(_ffmpegPath, GetFFprobeExecutable()), true);

            FFmpeg.SetExecutablesPath(_ffmpegPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ FFmpeg download failed: {ex.Message}");
            return false;
        }
    }

    private static string LocateDownloadedFile(string searchDirectory, string filename)
    {
        try
        {
            var files = Directory.GetFiles(searchDirectory, filename, SearchOption.AllDirectories);
            return files.Length > 0 ? files[0] : null;
        }
        catch
        {
            return null;
        }
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

    private static string GetFFmpegExecutable()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffmpeg.exe" : "ffmpeg";
    }

    private static string GetFFprobeExecutable()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffprobe.exe" : "ffprobe";
    }
}
