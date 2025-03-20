using System.Collections.Generic;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Interface for a media HLS playlist
    /// </summary>
    public interface IMediaPlaylist : IPlaylist
    {
        /// <summary>
        /// Gets the target duration of segments in this playlist in seconds
        /// </summary>
        double TargetDuration { get; }

        /// <summary>
        /// Gets a value indicating whether this playlist is endless (no EXT-X-ENDLIST tag)
        /// </summary>
        bool IsEndless { get; }

        /// <summary>
        /// Gets the media sequence number of the first segment
        /// </summary>
        int MediaSequence { get; }

        /// <summary>
        /// Gets a value indicating whether this playlist has any discontinuities
        /// </summary>
        bool HasDiscontinuity { get; }

        /// <summary>
        /// Gets the type of playlist (VOD, EVENT, etc.)
        /// </summary>
        string PlaylistType { get; }

        /// <summary>
        /// Gets the segments in the playlist
        /// </summary>
        IReadOnlyList<IMediaSegment> Segments { get; }

        /// <summary>
        /// Gets the URI of the I-frame playlist if available
        /// </summary>
        string IFramePlaylistUri { get; }
    }
} 