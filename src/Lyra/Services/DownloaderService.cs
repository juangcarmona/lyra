using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using VideoLibrary;

namespace Lyra.Services
{
    public class DownloaderService
    {
        private readonly ILogger<DownloaderService> _logger;
        private readonly ConversionService _conversionService;
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly string _downloadPath = Path.Combine(Environment.CurrentDirectory, "downloads");

        public DownloaderService(ILogger<DownloaderService> logger)
        {
            _logger = logger;
            Directory.CreateDirectory(_downloadPath);
            _conversionService = new ConversionService();
        }

        public async Task DownloadVideo(string url)
        {
            _logger.LogInformation($"🎬 Downloading video: {url}");

            try
            {
                var youtube = YouTube.Default;
                var video = await youtube.GetVideoAsync(url);
                string videoPath = Path.Combine(_downloadPath, video.FullName);

                await File.WriteAllBytesAsync(videoPath, await video.GetBytesAsync());

                _logger.LogInformation($"✅ Download complete: {videoPath}");

                // Convert to MP3
                await _conversionService.ConvertToMp3(videoPath);

                // Optional: Delete the original video file after conversion
                File.Delete(videoPath);
                _logger.LogInformation($"🗑️ Deleted original video file: {videoPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error downloading video: {ex.Message}");
            }
        }

        public async Task DownloadPlaylist(string playlistUrl)
        {
            _logger.LogInformation($"📜 Fetching playlist: {playlistUrl}");

            try
            {
                // Fetch the playlist webpage
                string htmlContent = await _httpClient.GetStringAsync(playlistUrl);

                // Extract video URLs using regex (matches video IDs from playlist page)
                var matches = Regex.Matches(htmlContent, @"watch\?v=(.{11})");
                var videoUrls = new HashSet<string>(); // Use HashSet to avoid duplicates

                foreach (Match match in matches)
                {
                    string videoId = match.Groups[1].Value;
                    string videoUrl = $"https://www.youtube.com/watch?v={videoId}";

                    if (videoUrls.Add(videoUrl)) // Adds only if it's unique
                    {
                        _logger.LogDebug($"🔗 Found video: {videoUrl}");
                    }
                }

                if (videoUrls.Count == 0)
                {
                    _logger.LogWarning("❌ No videos found in playlist.");
                    return;
                }

                _logger.LogInformation($"🔹 Found {videoUrls.Count} videos in playlist.");

                // Download each video
                foreach (var url in videoUrls)
                {
                    await DownloadVideo(url);
                }

                _logger.LogInformation($"✅ Playlist downloaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"⚠️ Error fetching playlist: {ex.Message}");
            }
        }
    }
}
