using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lyra.Services;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Lyra.Tests
{
    public class DownloaderServiceTests : IDisposable
    {
        private readonly DownloaderService _downloader;
        private readonly string _testDownloadPath;
        private readonly ILogger<DownloaderService> _mockLogger;
        private readonly ConversionService _mockConversionService;

        public DownloaderServiceTests()
        {
            // Create a unique temp test directory
            _testDownloadPath = Path.Combine(Path.GetTempPath(), "lyra_test_downloads_" + Guid.NewGuid());
            Directory.CreateDirectory(_testDownloadPath);

            _mockLogger = Substitute.For<ILogger<DownloaderService>>();
            _mockConversionService = Substitute.For<ConversionService>();

            // Inject the logger mock
            _downloader = new DownloaderService(_mockLogger, _mockConversionService);
        }

        [Fact]
        public async Task DownloadSingleVideo_ShouldDownloadAndConvertMp3()
        {
            string videoUrl = "https://www.youtube.com/watch?v=WBqf-vSMA6k"; // Example (verify it's available)

            await _downloader.DownloadAudio(videoUrl);

            // Assert the MP3 file exists
            string expectedMp3 = Directory.GetFiles(_testDownloadPath, "*.mp3").FirstOrDefault();
            expectedMp3.Should().NotBeNull("MP3 file should be created after conversion");

            _mockLogger.Received().LogInformation(Arg.Any<string>());

            Console.WriteLine($"✅ Test Passed: Downloaded {expectedMp3}");
        }

        [Fact]
        public async Task DownloadPlaylist_ShouldDownloadMultipleMp3s()
        {
            string playlistUrl = "https://www.youtube.com/playlist?list=PLquujPA7EWzOoUtojEcgQJCpTkdbVG4LV"; // Example

            await _downloader.DownloadPlaylistAudios(playlistUrl);

            // Assert multiple MP3 files exist
            var mp3Files = Directory.GetFiles(_testDownloadPath, "*.mp3");
            mp3Files.Length.Should().BeGreaterThan(1, "A playlist should contain multiple MP3 files");

            _mockLogger.Received().LogInformation(Arg.Any<string>());

            Console.WriteLine($"✅ Test Passed: Downloaded {mp3Files.Length} MP3 files from playlist");
        }

        public void Dispose()
        {
            // Cleanup test directory after tests
            if (Directory.Exists(_testDownloadPath))
            {
                Directory.Delete(_testDownloadPath, true);
            }
        }
    }
}
