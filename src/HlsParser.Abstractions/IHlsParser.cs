using System;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Interface for the HLS parser, providing methods to parse HLS playlists
    /// </summary>
    public interface IHlsParser
    {
        /// <summary>
        /// Parses a playlist, automatically detecting whether it's a master or media playlist
        /// </summary>
        /// <param name="content">The content of the playlist</param>
        /// <param name="uri">The URI of the playlist</param>
        /// <returns>The parsed <see cref="IPlaylist"/></returns>
        IPlaylist Parse(string content, Uri uri);

        /// <summary>
        /// Parses a master playlist
        /// </summary>
        /// <param name="content">The content of the playlist</param>
        /// <param name="uri">The URI of the playlist</param>
        /// <returns>The parsed <see cref="IMasterPlaylist"/></returns>
        IMasterPlaylist ParseMasterPlaylist(string content, Uri uri);

        /// <summary>
        /// Parses a media playlist
        /// </summary>
        /// <param name="content">The content of the playlist</param>
        /// <param name="uri">The URI of the playlist</param>
        /// <returns>The parsed <see cref="IMediaPlaylist"/></returns>
        IMediaPlaylist ParseMediaPlaylist(string content, Uri uri);
    }
} 