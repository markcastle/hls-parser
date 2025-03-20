using System.Collections.Generic;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Interface for a master HLS playlist
    /// </summary>
    public interface IMasterPlaylist : IPlaylist
    {
        /// <summary>
        /// Gets the stream variants in the master playlist
        /// </summary>
        IReadOnlyList<IStreamInfo> Streams { get; }

        /// <summary>
        /// Gets the rendition groups in the master playlist
        /// </summary>
        IReadOnlyList<IRenditionGroup> RenditionGroups { get; }
    }
} 