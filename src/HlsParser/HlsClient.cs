using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HlsParser.Models;
using HlsParser.Parsers;

namespace HlsParser
{
    /// <summary>
    /// Client for downloading and parsing HLS playlists
    /// </summary>
    public class HlsClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly PlaylistParser _parser;
        private readonly bool _ownsHttpClient;

        /// <summary>
        /// Creates a new instance of the HLSClient class
        /// </summary>
        public HlsClient() : this(new HttpClient(), true)
        {
        }

        /// <summary>
        /// Creates a new instance of the HLSClient class with the specified HttpClient
        /// </summary>
        /// <param name="httpClient">The HttpClient to use for downloading playlists and segments</param>
        /// <param name="ownsHttpClient">Indicates if the client owns the HttpClient and should dispose it</param>
        public HlsClient(HttpClient httpClient, bool ownsHttpClient = false)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _parser = new PlaylistParser();
            _ownsHttpClient = ownsHttpClient;
        }

        /// <summary>
        /// Gets a master playlist from the specified URI
        /// </summary>
        /// <param name="uri">The URI of the master playlist</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The master playlist</returns>
        public async Task<MasterPlaylist> GetMasterPlaylistAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            var playlist = await GetPlaylistAsync(uri, cancellationToken).ConfigureAwait(false);
            if (playlist is MasterPlaylist masterPlaylist)
            {
                return masterPlaylist;
            }

            throw new InvalidOperationException($"The playlist at {uri} is not a master playlist.");
        }

        /// <summary>
        /// Gets a media playlist from the specified URI
        /// </summary>
        /// <param name="uri">The URI of the media playlist</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The media playlist</returns>
        public async Task<MediaPlaylist> GetMediaPlaylistAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            var playlist = await GetPlaylistAsync(uri, cancellationToken).ConfigureAwait(false);
            if (playlist is MediaPlaylist mediaPlaylist)
            {
                return mediaPlaylist;
            }

            throw new InvalidOperationException($"The playlist at {uri} is not a media playlist.");
        }

        /// <summary>
        /// Gets a playlist from the specified URI
        /// </summary>
        /// <param name="uri">The URI of the playlist</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The playlist</returns>
        public async Task<Playlist> GetPlaylistAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            var content = await _httpClient.GetStringAsync(uri).ConfigureAwait(false);
            return _parser.Parse(content, uri);
        }

        /// <summary>
        /// Gets a segment from the specified URI
        /// </summary>
        /// <param name="segment">The segment to download</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The segment data</returns>
        public async Task<byte[]> GetSegmentAsync(MediaSegment segment, CancellationToken cancellationToken = default)
        {
            if (segment == null)
                throw new ArgumentNullException(nameof(segment));

            if (segment.Uri == null)
                throw new ArgumentException("Segment URI cannot be null.", nameof(segment));

            return await _httpClient.GetByteArrayAsync(segment.Uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Disposes the resources used by the client
        /// </summary>
        public void Dispose()
        {
            if (_ownsHttpClient)
            {
                _httpClient?.Dispose();
            }
        }
    }
} 