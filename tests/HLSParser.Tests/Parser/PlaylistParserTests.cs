using HlsParser.Models;
using HlsParser.Parsers;

namespace HlsParser.Tests.Parser
{
    public class PlaylistParserTests
    {
        [Fact]
        public void Parse_ValidMasterPlaylist_ReturnsMasterPlaylist()
        {
            // Arrange
            PlaylistParser? parser = new();
            string? content = @"#EXTM3U
#EXT-X-VERSION:4
#EXT-X-STREAM-INF:BANDWIDTH=1280000,RESOLUTION=720x480,CODECS=""avc1.66.30,mp4a.40.2""
http://example.com/low.m3u8
#EXT-X-STREAM-INF:BANDWIDTH=2560000,RESOLUTION=1280x720,CODECS=""avc1.77.30,mp4a.40.2""
http://example.com/mid.m3u8
#EXT-X-STREAM-INF:BANDWIDTH=7680000,RESOLUTION=1920x1080,CODECS=""avc1.77.30,mp4a.40.2""
http://example.com/hi.m3u8";
            Uri? uri = new("http://example.com/master.m3u8");

            // Act
            MasterPlaylist? playlist = parser.Parse(content, uri) as MasterPlaylist;

            // Assert
            Assert.NotNull(playlist);
            Assert.Equal(4, playlist.Version);
            Assert.Equal(3, playlist.Streams.Count);
            Assert.Equal(1280000, playlist.Streams[0].Bandwidth);
            Assert.Equal("720x480", playlist.Streams[0].Resolution);
            Assert.Equal("avc1.66.30,mp4a.40.2", playlist.Streams[0].Codecs);
            Assert.Equal(new Uri("http://example.com/low.m3u8"), playlist.Streams[0].Uri);
        }

        [Fact]
        public void Parse_ValidMediaPlaylist_ReturnsMediaPlaylist()
        {
            // Arrange
            PlaylistParser? parser = new();
            string? content = @"#EXTM3U
#EXT-X-VERSION:3
#EXT-X-TARGETDURATION:8
#EXT-X-MEDIA-SEQUENCE:2680

#EXTINF:7.975,
https://example.com/segment2680.ts
#EXTINF:7.941,
https://example.com/segment2681.ts
#EXTINF:7.975,
https://example.com/segment2682.ts";
            Uri? uri = new("http://example.com/playlist.m3u8");

            // Act
            MediaPlaylist? playlist = parser.Parse(content, uri) as MediaPlaylist;

            // Assert
            Assert.NotNull(playlist);
            Assert.Equal(3, playlist.Version);
            Assert.Equal(8, playlist.TargetDuration);
            Assert.Equal(2680, playlist.MediaSequence);
            Assert.Equal(3, playlist.Segments.Count);
            Assert.Equal(7.975, playlist.Segments[0].Duration);
            Assert.True(playlist.IsEndless);
        }

        [Fact]
        public void Parse_MediaPlaylistWithEndTag_SetsIsEndlessFalse()
        {
            // Arrange
            PlaylistParser? parser = new();
            string? content = @"#EXTM3U
#EXT-X-VERSION:3
#EXT-X-TARGETDURATION:8
#EXT-X-MEDIA-SEQUENCE:2680

#EXTINF:7.975,
https://example.com/segment2680.ts
#EXTINF:7.941,
https://example.com/segment2681.ts
#EXTINF:7.975,
https://example.com/segment2682.ts
#EXT-X-ENDLIST";
            Uri? uri = new("http://example.com/playlist.m3u8");

            // Act
            MediaPlaylist? playlist = parser.Parse(content, uri) as MediaPlaylist;

            // Assert
            Assert.NotNull(playlist);
            Assert.False(playlist.IsEndless);
        }

        [Fact]
        public void Parse_InvalidPlaylistFormat_ThrowsFormatException()
        {
            // Arrange
            PlaylistParser? parser = new();
            string? content = "This is not a valid playlist";
            Uri? uri = new("http://example.com/playlist.m3u8");

            // Act & Assert
            Assert.Throws<FormatException>(() => parser.Parse(content, uri));
        }

        [Fact]
        public void Parse_NullContent_ThrowsArgumentException()
        {
            // Arrange
            PlaylistParser? parser = new();
            Uri? uri = new("http://example.com/playlist.m3u8");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => parser.Parse(null, uri));
        }

        [Fact]
        public void Parse_NullUri_ThrowsArgumentNullException()
        {
            // Arrange
            PlaylistParser? parser = new();
            string? content = "#EXTM3U\n#EXT-X-VERSION:3";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => parser.Parse(content, null));
        }
    }
} 