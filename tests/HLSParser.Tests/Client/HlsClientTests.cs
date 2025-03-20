using Moq;
using Moq.Protected;
using System.Net;
using HlsParser.Models;

namespace HlsParser.Tests.Client
{
    public class HlsClientTests
    {
        [Fact]
        public async Task GetMasterPlaylistAsync_ValidUri_ReturnsMasterPlaylist()
        {
            // Arrange
            string? content = @"#EXTM3U
#EXT-X-VERSION:4
#EXT-X-STREAM-INF:BANDWIDTH=1280000,RESOLUTION=720x480,CODECS=""avc1.66.30,mp4a.40.2""
http://example.com/low.m3u8";
            Uri? uri = new("http://example.com/master.m3u8");
            Mock<HttpMessageHandler>? mockHttpMessageHandler = SetupMockHttpMessageHandler(uri, content);
            HttpClient? httpClient = new(mockHttpMessageHandler.Object);
            HlsClient? client = new(httpClient);

            // Act
            MasterPlaylist? playlist = await client.GetMasterPlaylistAsync(uri);

            // Assert
            Assert.NotNull(playlist);
            Assert.Equal(4, playlist.Version);
            Assert.Single(playlist.Streams);
            Assert.Equal(1280000, playlist.Streams[0].Bandwidth);
        }

        [Fact]
        public async Task GetMediaPlaylistAsync_ValidUri_ReturnsMediaPlaylist()
        {
            // Arrange
            string? content = @"#EXTM3U
#EXT-X-VERSION:3
#EXT-X-TARGETDURATION:8
#EXT-X-MEDIA-SEQUENCE:2680

#EXTINF:7.975,
https://example.com/segment2680.ts";
            Uri? uri = new("http://example.com/playlist.m3u8");
            Mock<HttpMessageHandler>? mockHttpMessageHandler = SetupMockHttpMessageHandler(uri, content);
            HttpClient? httpClient = new(mockHttpMessageHandler.Object);
            HlsClient? client = new(httpClient);

            // Act
            MediaPlaylist? playlist = await client.GetMediaPlaylistAsync(uri);

            // Assert
            Assert.NotNull(playlist);
            Assert.Equal(3, playlist.Version);
            Assert.Equal(8, playlist.TargetDuration);
            Assert.Equal(2680, playlist.MediaSequence);
            Assert.Single(playlist.Segments);
            Assert.Equal(7.975, playlist.Segments[0].Duration);
        }

        [Fact]
        public async Task GetPlaylistAsync_InvalidUri_ThrowsHttpRequestException()
        {
            // Arrange
            Uri? uri = new("http://example.com/nonexistent.m3u8");
            Mock<HttpMessageHandler>? mockHttpMessageHandler = new();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException());
            HttpClient? httpClient = new(mockHttpMessageHandler.Object);
            HlsClient? client = new(httpClient);

            // Act & Assert
            HttpRequestException exception = await Assert.ThrowsAsync<HttpRequestException>(() => client.GetPlaylistAsync(uri));
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetSegmentAsync_ValidSegment_ReturnsSegmentData()
        {
            // Arrange
            Uri? segmentUri = new("http://example.com/segment.ts");
            MediaSegment? segment = new() { Uri = segmentUri };
            byte[]? segmentData = new byte[] { 0x00, 0x01, 0x02, 0x03 };

            Mock<HttpMessageHandler>? mockHttpMessageHandler = new();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.RequestUri == segmentUri),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ByteArrayContent(segmentData)
                });

            HttpClient? httpClient = new(mockHttpMessageHandler.Object);
            HlsClient? client = new(httpClient);

            // Act
            byte[]? result = await client.GetSegmentAsync(segment);

            // Assert
            Assert.Equal(segmentData, result);
        }

        [Fact]
        public async Task GetSegmentAsync_NullSegment_ThrowsArgumentNullException()
        {
            // Arrange
            HlsClient? client = new();

            // Act & Assert
            ArgumentNullException exception = await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetSegmentAsync(null));
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetSegmentAsync_SegmentWithNullUri_ThrowsArgumentException()
        {
            // Arrange
            MediaSegment? segment = new() { Uri = null };
            HlsClient? client = new();

            // Act & Assert
            ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(() => client.GetSegmentAsync(segment));
            Assert.NotNull(exception);
        }

        private Mock<HttpMessageHandler> SetupMockHttpMessageHandler(Uri uri, string content)
        {
            Mock<HttpMessageHandler>? mockHttpMessageHandler = new();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.RequestUri == uri),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content)
                });

            return mockHttpMessageHandler;
        }
    }
} 