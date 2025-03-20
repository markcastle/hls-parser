using System;
using System.Collections.Generic;

namespace HlsParser.Models
{
    /// <summary>
    /// Base class for HLS playlists
    /// </summary>
    public abstract class Playlist
    {
        /// <summary>
        /// The HLS protocol version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// The URI of the playlist
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// The tags associated with this playlist
        /// </summary>
        public IList<Tag> Tags { get; set; } = new List<Tag>();
    }
} 