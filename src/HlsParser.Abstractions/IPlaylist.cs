using System;
using System.Collections.Generic;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Interface for a generic HLS playlist
    /// </summary>
    public interface IPlaylist
    {
        /// <summary>
        /// Gets the version of the playlist
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Gets the URI of the playlist
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Gets the tags in the playlist
        /// </summary>
        IReadOnlyList<ITag> Tags { get; }
    }
} 