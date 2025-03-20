using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using HlsParser.Abstractions;

namespace HlsParser.Samples.DependencyInjection
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Set up the dependency injection container
            var services = new ServiceCollection();
            
            // Add HlsParser services to the container
            services.AddHlsParser();
            
            // Build the service provider
            using var serviceProvider = services.BuildServiceProvider();
            
            // Resolve the HLS client from the container
            var hlsClient = serviceProvider.GetRequiredService<IHlsClient>();
            
            try
            {
                // Use a sample HLS playlist URL
                string playlistUrl = "https://bitdash-a.akamaihd.net/content/sintel/hls/playlist.m3u8";
                
                if (args.Length > 0)
                {
                    playlistUrl = args[0];
                }
                
                Console.WriteLine($"Analyzing HLS playlist: {playlistUrl}");
                Console.WriteLine();
                
                // Parse the playlist
                var playlist = await hlsClient.GetPlaylistAsync(new Uri(playlistUrl));
                
                // Check if it's a master playlist
                if (playlist is IMasterPlaylist masterPlaylist)
                {
                    Console.WriteLine("Master Playlist found!");
                    Console.WriteLine($"Version: {masterPlaylist.Version}");
                    Console.WriteLine($"Streams: {masterPlaylist.Streams.Count}");
                    
                    // Display info about each stream
                    for (int i = 0; i < masterPlaylist.Streams.Count; i++)
                    {
                        var stream = masterPlaylist.Streams[i];
                        Console.WriteLine($"Stream #{i + 1}:");
                        Console.WriteLine($"  Bandwidth: {stream.Bandwidth / 1000.0:F0} kbps");
                        Console.WriteLine($"  Resolution: {stream.Resolution}");
                        Console.WriteLine($"  Codecs: {stream.Codecs}");
                        
                        if (!string.IsNullOrEmpty(stream.AudioGroupId))
                        {
                            Console.WriteLine($"  Audio Group: {stream.AudioGroupId}");
                        }
                        
                        if (stream.FrameRate.HasValue)
                        {
                            Console.WriteLine($"  Frame Rate: {stream.FrameRate.Value:F3}");
                        }
                        
                        Console.WriteLine();
                    }
                    
                    // Display info about rendition groups
                    foreach (var renditionGroup in masterPlaylist.RenditionGroups)
                    {
                        Console.WriteLine($"{renditionGroup.Type} Rendition Group: {renditionGroup.GroupId}");
                        Console.WriteLine($"Renditions: {renditionGroup.Renditions.Count}");
                        
                        foreach (var rendition in renditionGroup.Renditions)
                        {
                            Console.WriteLine($"  {rendition.Name} ({rendition.Language})");
                            Console.WriteLine($"    Default: {rendition.IsDefault}, Autoselect: {rendition.Autoselect}");
                            
                            if (rendition.Uri != null)
                            {
                                Console.WriteLine($"    URI: {rendition.Uri}");
                            }
                            
                            Console.WriteLine();
                        }
                    }
                }
                // Check if it's a media playlist
                else if (playlist is IMediaPlaylist mediaPlaylist)
                {
                    Console.WriteLine("Media Playlist found!");
                    Console.WriteLine($"Version: {mediaPlaylist.Version}");
                    Console.WriteLine($"Target Duration: {mediaPlaylist.TargetDuration:F1} seconds");
                    Console.WriteLine($"Media Sequence: {mediaPlaylist.MediaSequence}");
                    Console.WriteLine($"Endless: {mediaPlaylist.IsEndless}");
                    Console.WriteLine($"Playlist Type: {mediaPlaylist.PlaylistType}");
                    Console.WriteLine($"Has Discontinuity: {mediaPlaylist.HasDiscontinuity}");
                    Console.WriteLine($"Segments: {mediaPlaylist.Segments.Count}");
                    
                    Console.WriteLine("\nSegment Preview (first 5):");
                    
                    // Display info about the first 5 segments
                    for (int i = 0; i < Math.Min(5, mediaPlaylist.Segments.Count); i++)
                    {
                        var segment = mediaPlaylist.Segments[i];
                        Console.WriteLine($"Segment #{i + 1} (#{segment.SequenceNumber}):");
                        Console.WriteLine($"  Duration: {segment.Duration:F1} seconds");
                        Console.WriteLine($"  URI: {segment.Uri}");
                        
                        if (segment.Encryption != null && segment.Encryption.Method != "NONE")
                        {
                            Console.WriteLine($"  Encryption: {segment.Encryption.Method}");
                            Console.WriteLine($"  Key URI: {segment.Encryption.KeyUri}");
                        }
                        
                        if (segment.ProgramDateTime.HasValue)
                        {
                            Console.WriteLine($"  Program Date Time: {segment.ProgramDateTime.Value}");
                        }
                        
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
