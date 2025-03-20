using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HlsParser;
using HlsParser.Models;

namespace HlsParser.CommandLine
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("📺 HLS Parser Command Line Tool");
            Console.WriteLine("==============================");
            
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: dotnet run -- <playlist_url>");
                Console.WriteLine("Example: dotnet run -- https://example.com/master.m3u8");
                return;
            }

            string url = args[0];
            
            try
            {
                await AnalyzePlaylist(url);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.ResetColor();
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        static async Task AnalyzePlaylist(string url)
        {
            Console.WriteLine($"🔍 Analyzing playlist: {url}");
            
            using var client = new HlsClient();
            var uri = new Uri(url);
            
            try
            {
                var masterPlaylist = await client.GetMasterPlaylistAsync(uri);
                await DisplayMasterPlaylistInfo(client, masterPlaylist);
            }
            catch (InvalidOperationException)
            {
                // If it's not a master playlist, try as a media playlist
                var mediaPlaylist = await client.GetMediaPlaylistAsync(uri);
                DisplayMediaPlaylistInfo(mediaPlaylist);
            }
        }

        static async Task DisplayMasterPlaylistInfo(HlsClient client, MasterPlaylist playlist)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n🎬 MASTER PLAYLIST");
            Console.ResetColor();
            
            Console.WriteLine($"Version: {playlist.Version}");
            Console.WriteLine($"URI: {playlist.Uri}");
            
            Console.WriteLine($"\n📊 Stream Variants: {playlist.Streams.Count}");
            
            var sortedStreams = playlist.Streams.OrderByDescending(s => s.Bandwidth).ToList();
            
            for (int i = 0; i < sortedStreams.Count; i++)
            {
                var stream = sortedStreams[i];
                Console.WriteLine($"\n  Stream #{i+1}:");
                Console.WriteLine($"  Resolution: {stream.Resolution}");
                Console.WriteLine($"  Bandwidth: {stream.Bandwidth/1000} kbps");
                if (stream.AverageBandwidth.HasValue)
                {
                    Console.WriteLine($"  Average Bandwidth: {stream.AverageBandwidth.Value/1000} kbps");
                }
                Console.WriteLine($"  Codecs: {stream.Codecs}");
                if (stream.FrameRate.HasValue)
                {
                    Console.WriteLine($"  Frame Rate: {stream.FrameRate.Value} fps");
                }
                
                // You can uncomment this to display the media playlist for each stream variant
                // but it might generate a lot of output.
                /*
                try
                {
                    Console.WriteLine($"  Media Playlist:");
                    var mediaPlaylist = await client.GetMediaPlaylistAsync(stream.Uri);
                    Console.WriteLine($"    Segments: {mediaPlaylist.Segments.Count}");
                    Console.WriteLine($"    Target Duration: {mediaPlaylist.TargetDuration} seconds");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ❌ Failed to load media playlist: {ex.Message}");
                }
                */
            }
            
            if (playlist.RenditionGroups.Count > 0)
            {
                Console.WriteLine($"\n🔊 Rendition Groups: {playlist.RenditionGroups.Count}");
                
                foreach (var group in playlist.RenditionGroups)
                {
                    Console.WriteLine($"\n  Group Type: {group.Type}");
                    Console.WriteLine($"  Group ID: {group.GroupId}");
                    Console.WriteLine($"  Renditions: {group.Renditions.Count}");
                    
                    foreach (var rendition in group.Renditions)
                    {
                        Console.WriteLine($"    Name: {rendition.Name}");
                        if (!string.IsNullOrEmpty(rendition.Language))
                        {
                            Console.WriteLine($"    Language: {rendition.Language}");
                        }
                        Console.WriteLine($"    Default: {rendition.Default}");
                        Console.WriteLine($"    Autoselect: {rendition.Autoselect}");
                    }
                }
            }
            
            // Display all tags
            Console.WriteLine("\n🏷️ Tags:");
            foreach (var tag in playlist.Tags)
            {
                Console.WriteLine($"  {tag.Name}: {tag.RawValue}");
            }
        }

        static void DisplayMediaPlaylistInfo(MediaPlaylist playlist)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n📝 MEDIA PLAYLIST");
            Console.ResetColor();
            
            Console.WriteLine($"Version: {playlist.Version}");
            Console.WriteLine($"URI: {playlist.Uri}");
            Console.WriteLine($"Target Duration: {playlist.TargetDuration} seconds");
            Console.WriteLine($"Media Sequence: {playlist.MediaSequence}");
            Console.WriteLine($"Endless: {playlist.IsEndless}");
            
            if (!string.IsNullOrEmpty(playlist.PlaylistType))
            {
                Console.WriteLine($"Playlist Type: {playlist.PlaylistType}");
            }
            
            Console.WriteLine($"Has Discontinuity: {playlist.HasDiscontinuity}");
            
            Console.WriteLine($"\n🎞️ Segments: {playlist.Segments.Count}");
            
            int displayLimit = Math.Min(playlist.Segments.Count, 10); // Limit to 10 segments for display
            
            for (int i = 0; i < displayLimit; i++)
            {
                var segment = playlist.Segments[i];
                Console.WriteLine($"\n  Segment #{i+1}:");
                Console.WriteLine($"  Duration: {segment.Duration} seconds");
                Console.WriteLine($"  Sequence: {segment.SequenceNumber}");
                Console.WriteLine($"  URI: {segment.Uri}");
                
                if (segment.HasDiscontinuity)
                {
                    Console.WriteLine($"  Has Discontinuity: {segment.HasDiscontinuity}");
                }
                
                if (!string.IsNullOrEmpty(segment.ByteRange))
                {
                    Console.WriteLine($"  Byte Range: {segment.ByteRange}");
                }
                
                if (segment.ProgramDateTime.HasValue)
                {
                    Console.WriteLine($"  Program Date Time: {segment.ProgramDateTime.Value}");
                }
                
                if (segment.Encryption != null)
                {
                    Console.WriteLine($"  Encryption Method: {segment.Encryption.Method}");
                    Console.WriteLine($"  Key URI: {segment.Encryption.KeyUri}");
                }
            }
            
            if (playlist.Segments.Count > displayLimit)
            {
                Console.WriteLine($"\n  ... {playlist.Segments.Count - displayLimit} more segments ...");
            }
            
            // Display all tags
            Console.WriteLine("\n🏷️ Tags:");
            foreach (var tag in playlist.Tags)
            {
                Console.WriteLine($"  {tag.Name}: {tag.RawValue}");
            }
        }
    }
}
