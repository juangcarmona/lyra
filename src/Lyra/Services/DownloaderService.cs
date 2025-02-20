using Microsoft.Extensions.Logging;
using YoutubeExplode;

namespace Lyra.Services
{
    public class DownloaderService
    {
        private readonly ILogger<DownloaderService> _logger;
        private readonly ConversionService _conversionService;
        private static readonly HttpClient _httpClient = new();
        private readonly YoutubeClient _youtube;
        private readonly bool _convertToMp3;
        private readonly string _destinationPath;

        public DownloaderService(
            ILogger<DownloaderService> logger,
            ConversionService conversionService,
            bool convertToMp3,
            string destinationPath)
        {
            _logger = logger;
            _conversionService = conversionService;
            _youtube = new YoutubeClient();
            _convertToMp3 = convertToMp3;
            _destinationPath = destinationPath;

            Directory.CreateDirectory(_destinationPath);
        }

        public async Task DownloadAudio(string url)
        {
            try
            {
                _logger.LogInformation($"🎵 Downloading audio: {url}");

                var video = await _youtube.Videos.GetAsync(url);
                var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(url);

                var audioStreamInfo = streamManifest
                    .GetAudioOnlyStreams()
                    .OrderByDescending(s => s.Bitrate)
                    .FirstOrDefault();

                if (audioStreamInfo == null)
                {
                    _logger.LogError("❌ No audio streams found.");
                    return;
                }

                string safeTitle = FileNameSanitizer.SanitizeFileName(video.Title);
                string audioPath = Path.Combine(_destinationPath, safeTitle + "." + audioStreamInfo.Container.Name);

                await _youtube.Videos.Streams.DownloadAsync(audioStreamInfo, audioPath);

                _logger.LogInformation($"✅ Audio downloaded: {audioPath}");

                if (_convertToMp3)
                {
                    await _conversionService.ConvertToMp3(audioPath);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error downloading audio: {ex.Message}");
            }
        }

        public async Task DownloadPlaylistAudios(string playlistUrl)
        {
            _logger.LogInformation($"📜 Fetching playlist: {playlistUrl}");

            var videoUrls = new List<string>();

            try
            {
                // Fetch videos asynchronously using await foreach
                await foreach (var video in _youtube.Playlists.GetVideosAsync(playlistUrl))
                {
                    string videoUrl = $"https://www.youtube.com/watch?v={video.Id}";
                    videoUrls.Add(videoUrl);
                    _logger.LogDebug($"🔗 Found video: {videoUrl}");
                }

                if (!videoUrls.Any())
                {
                    _logger.LogWarning("❌ No videos found in the playlist.");
                    return;
                }

                _logger.LogInformation($"🔹 Found {videoUrls.Count} videos in the playlist. Starting downloads...");

                // **Execute all downloads in parallel**
                var downloadTasks = videoUrls.Select(url => DownloadAudio(url));
                await Task.WhenAll(downloadTasks);

                _logger.LogInformation($"✅ Playlist download and conversion completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"⚠️ Error fetching playlist: {ex.Message}");
            }
        }




    }
}
