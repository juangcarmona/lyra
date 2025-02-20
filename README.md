# 𝐋𝐘𝐑𝐀 - 𝐋ightweight 𝐘outube 𝐑ipping 𝐀ssistant

![LYRA Mosaic](assets/lyra.webp)

## Introduction

𝐋𝐘𝐑𝐀 (𝐋ightweight 𝐘outube 𝐑ipping 𝐀ssistant) is a modern and efficient command-line tool designed to extract and download audio from YouTube videos and playlists. It is inspired by a previous project, [**YoutubeMp3Downloader**](https://github.com/juangcarmona/youtube-mp3-downloader), which was an early attempt at building a similar tool. While that project had its strengths, LYRA takes things further by offering a streamlined CLI experience, enhanced performance, and playlist support.

## Features
- **Download a single YouTube video as MP3**.
- **Download all audio files from a playlist**.
- **Fast, lightweight, and simple to use**.
- **Cross-platform**: Works on Linux, macOS, and Windows.
- **FFmpeg integration** for high-quality audio conversion.

## Installation

### Linux/macOS
```bash
curl -sSL https://github.com/juangcarmona/lyra/main/scripts/install.sh | bash
```

### Windows (PowerShell)
```powershell
iex ((New-Object System.Net.WebClient).DownloadString('https://github.com/juangcarmona/lyra/main/scripts/install.ps1'))
```

## Usage

### Download a single YouTube video as MP3
```bash
lyra https://www.youtube.com/watch?v=YOUR_VIDEO_ID
```

### Download all audio files from a playlist
```bash
lyra -p https://www.youtube.com/playlist?list=YOUR_PLAYLIST_ID
```

## Roadmap
- [ ] Support single video downloads
- [ ] Implement playlist downloading
- [ ] Add multi-threaded downloads
- [ ] Improve metadata handling (ID3 tags, album art)
- [ ] Provide binary releases for easy installation

## Contributing
Contributions are welcome! Feel free to submit issues, feature requests, or pull requests to improve LYRA.

## License
LYRA is licensed under the Apache 2.0 License. See the [LICENSE](LICENSE) file for more details.

---

**LYRA - Extracting music from the web, effortlessly.** 🎶