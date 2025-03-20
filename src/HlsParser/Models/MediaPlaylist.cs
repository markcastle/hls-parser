using System.Collections.Generic;

namespace HlsParser.Models
{
    /// <summary>
    /// Represents an HLS media playlist containing media segments
    /// </summary>
    public class MediaPlaylist : Playlist
    {
        /// <summary>
        /// The target duration in seconds for segments in this playlist
        /// </summary>
        public double TargetDuration { get; set; }

        /// <summary>
        /// Indicates if the playlist is endless (does not have an EXT-X-ENDLIST tag)
        /// </summary>
        public bool IsEndless { get; set; }

        /// <summary>
        /// The media sequence number of the first segment in the playlist
        /// </summary>
        public int MediaSequence { get; set; }

        /// <summary>
        /// Indicates if the segments in this playlist are discontinuous
        /// </summary>
        public bool HasDiscontinuity { get; set; }

        /// <summary>
        /// The playlist type if specified (VOD, EVENT, etc.)
        /// </summary>
        public string PlaylistType { get; set; }

        /// <summary>
        /// The media segments in this playlist
        /// </summary>
        public IList<MediaSegment> Segments { get; set; } = new List<MediaSegment>();

        /// <summary>
        /// The I-frame only playlist URI if present
        /// </summary>
        public string FramePlaylistUri { get; set; }
    }
} 