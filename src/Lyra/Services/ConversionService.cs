using Xabe.FFmpeg;
using Microsoft.Extensions.Logging;
using Lyra.Utils;

public class ConversionService
{
    private readonly ILogger<ConversionService> _logger;

    public ConversionService(ILogger<ConversionService> logger)
    {
        _logger = logger;
        FFmpegChecker.EnsureFFmpegIsAvailable(_logger);
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
}
