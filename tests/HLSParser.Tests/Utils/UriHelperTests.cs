using System;
using HlsParser.Utils;
using Xunit;

namespace HlsParser.Tests.Utils
{
    public class UriHelperTests
    {
        [Fact]
        public void ResolveUri_RelativeUriWithBaseUri_ReturnsAbsoluteUri()
        {
            // Arrange
            Uri baseUri = new("http://example.com/path/");
            string relativeUri = "file.m3u8";

            // Act
            Uri result = UriHelper.ResolveUri(baseUri, relativeUri);

            // Assert
            Assert.Equal(new Uri("http://example.com/path/file.m3u8"), result);
        }

        [Fact]
        public void ResolveUri_AbsoluteUriWithBaseUri_ReturnsOriginalUri()
        {
            // Arrange
            Uri baseUri = new("http://example.com/path/");
            string relativeUri = "http://other.com/file.m3u8";

            // Act
            Uri result = UriHelper.ResolveUri(baseUri, relativeUri);

            // Assert
            Assert.Equal(new Uri("http://other.com/file.m3u8"), result);
        }

        [Fact]
        public void ResolveUri_NullRelativeUri_ReturnsBaseUri()
        {
            // Arrange
            Uri baseUri = new("http://example.com/path/");

            // Act
            Uri result = UriHelper.ResolveUri(baseUri, null);

            // Assert
            Assert.Equal(baseUri, result);
        }

        [Fact]
        public void ResolveUri_EmptyRelativeUri_ReturnsBaseUri()
        {
            // Arrange
            Uri baseUri = new("http://example.com/path/");

            // Act
            Uri result = UriHelper.ResolveUri(baseUri, string.Empty);

            // Assert
            Assert.Equal(baseUri, result);
        }

        [Fact]
        public void ResolveUri_NullBaseUri_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => UriHelper.ResolveUri(null, "file.m3u8"));
        }

        [Fact]
        public void GetBaseUri_UriWithPath_ReturnsBaseUri()
        {
            // Arrange
            Uri uri = new("http://example.com/path/file.m3u8");

            // Act
            Uri result = UriHelper.GetBaseUri(uri);

            // Assert
            Assert.Equal(new Uri("http://example.com/path/"), result);
        }

        [Fact]
        public void GetBaseUri_UriWithoutPath_ReturnsSameUri()
        {
            // Arrange
            Uri uri = new("http://example.com/");

            // Act
            Uri result = UriHelper.GetBaseUri(uri);

            // Assert
            Assert.Equal(uri, result);
        }

        [Fact]
        public void GetBaseUri_NullUri_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => UriHelper.GetBaseUri(null));
        }

        [Fact]
        public void GetFileName_UriWithPath_ReturnsFileName()
        {
            // Arrange
            Uri uri = new("http://example.com/path/file.m3u8");

            // Act
            string result = UriHelper.GetFileName(uri);

            // Assert
            Assert.Equal("file.m3u8", result);
        }

        [Fact]
        public void GetFileName_UriWithoutPath_ReturnsEmptyString()
        {
            // Arrange
            Uri uri = new("http://example.com/");

            // Act
            string result = UriHelper.GetFileName(uri);

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void GetFileName_NullUri_ThrowsArgumentNullException()
        {
            // Act & Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => UriHelper.GetFileName(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }
} 