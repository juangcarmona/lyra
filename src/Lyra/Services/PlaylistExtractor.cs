// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using YoutubeExplode;
// using YoutubeExplode.Common;
// using YoutubeExplode.Playlists;
// using VideoLibrary;

// namespace Lyra.Services
// {
//     public class PlaylistExtractor
//     {
//         private readonly YoutubeClient _youtubeClient = new YoutubeClient();

//         public async Task<List<YouTubeVideo>> GetVideosFromPlaylist(string playlistUrl)
//         {
//             var videos = new List<YouTubeVideo>();

//             try
//             {
//                 var playlist = await _youtubeClient.Playlists.GetAsync(playlistUrl);
//                 var videoUrls = playlist.Videos.Where(v => v.Status == VideoStatus.Ok).Select(v => v.Url).ToList();

//                 foreach (var url in videoUrls)
//                 {
//                     var youtube = YouTube.Default; 
//                     var video = await youtube.GetVideoAsync(url);
//                     videos.Add(video);
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"❌ Error retrieving playlist: {ex.Message}");
//             }

//             return videos;
//         }
//     }
// }
