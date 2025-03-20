using System.Collections.Generic;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Interface for a rendition group in a master playlist
    /// </summary>
    public interface IRenditionGroup
    {
        /// <summary>
        /// Gets the type of the rendition group (AUDIO, SUBTITLES, etc.)
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the group ID
        /// </summary>
        string GroupId { get; }

        /// <summary>
        /// Gets the renditions in this group
        /// </summary>
        IReadOnlyList<IRendition> Renditions { get; }
    }
} 