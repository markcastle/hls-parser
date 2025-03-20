using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Adapter for the HlsClient class that implements the IHlsClient interface
    /// </summary>
    internal class HlsClientAdapter : IHlsClient
    {
        private readonly HlsClient _client;
        private readonly IHlsParser _parser;
        private readonly bool _ownsClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HlsClientAdapter"/> class
        /// </summary>
        /// <param name="httpClient">The HttpClient to use</param>
        /// <param name="parser">The HLS parser to use</param>
        public HlsClientAdapter(HttpClient httpClient, IHlsParser parser)
        {
            _client = new HlsClient(httpClient);
            _parser = parser;
            _ownsClient = false;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_ownsClient)
            {
                _client.Dispose();
            }
        }

        /// <inheritdoc/>
        public async Task<IMasterPlaylist> GetMasterPlaylistAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            var masterPlaylist = await _client.GetMasterPlaylistAsync(uri, cancellationToken).ConfigureAwait(false);
            return new MasterPlaylistAdapter(masterPlaylist);
        }

        /// <inheritdoc/>
        public async Task<IMediaPlaylist> GetMediaPlaylistAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            var mediaPlaylist = await _client.GetMediaPlaylistAsync(uri, cancellationToken).ConfigureAwait(false);
            return new MediaPlaylistAdapter(mediaPlaylist);
        }

        /// <inheritdoc/>
        public async Task<IPlaylist> GetPlaylistAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            var playlist = await _client.GetPlaylistAsync(uri, cancellationToken).ConfigureAwait(false);
            
            if (playlist is Models.MasterPlaylist masterPlaylist)
            {
                return new MasterPlaylistAdapter(masterPlaylist);
            }
            else if (playlist is Models.MediaPlaylist mediaPlaylist)
            {
                return new MediaPlaylistAdapter(mediaPlaylist);
            }
            
            throw new InvalidOperationException("Unknown playlist type");
        }

        /// <inheritdoc/>
        public async Task<byte[]> GetSegmentAsync(IMediaSegment segment, CancellationToken cancellationToken = default)
        {
            if (segment == null)
            {
                throw new ArgumentNullException(nameof(segment));
            }

            // Create a MediaSegment from the IMediaSegment interface
            var mediaSegment = new Models.MediaSegment
            {
                Uri = segment.Uri,
                Duration = segment.Duration,
                Title = segment.Title,
                SequenceNumber = segment.SequenceNumber,
                HasDiscontinuity = segment.HasDiscontinuity,
                ByteRange = segment.ByteRange,
                ProgramDateTime = segment.ProgramDateTime
            };

            // Handle encryption if present
            if (segment.Encryption != null)
            {
                mediaSegment.Encryption = new Models.EncryptionInfo
                {
                    Method = segment.Encryption.Method,
                    KeyUri = segment.Encryption.KeyUri,
                    Iv = segment.Encryption.Iv,
                    KeyFormat = segment.Encryption.KeyFormat,
                    KeyFormatVersions = segment.Encryption.KeyFormatVersions
                };
            }

            return await _client.GetSegmentAsync(mediaSegment, cancellationToken).ConfigureAwait(false);
        }
    }
} 