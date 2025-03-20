using System;
using System.Threading;
using System.Threading.Tasks;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Interface for the HLS client, providing methods to retrieve and parse HLS playlists and segments
    /// </summary>
    public interface IHlsClient : IDisposable
    {
        /// <summary>
        /// Gets a master playlist from the specified URI
        /// </summary>
        /// <param name="uri">The URI of the master playlist</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A <see cref="Task"/> containing the parsed <see cref="IMasterPlaylist"/></returns>
        Task<IMasterPlaylist> GetMasterPlaylistAsync(Uri uri, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a media playlist from the specified URI
        /// </summary>
        /// <param name="uri">The URI of the media playlist</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A <see cref="Task"/> containing the parsed <see cref="IMediaPlaylist"/></returns>
        Task<IMediaPlaylist> GetMediaPlaylistAsync(Uri uri, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a playlist from the specified URI, automatically detecting whether it's a master or media playlist
        /// </summary>
        /// <param name="uri">The URI of the playlist</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A <see cref="Task"/> containing the parsed <see cref="IPlaylist"/></returns>
        Task<IPlaylist> GetPlaylistAsync(Uri uri, CancellationToken cancellationToken = default);
    }
} 