using System.Collections.Generic;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Interface for a tag in an HLS playlist
    /// </summary>
    public interface ITag
    {
        /// <summary>
        /// Gets the name of the tag (e.g., "EXT-X-VERSION", "EXTINF")
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the raw value of the tag
        /// </summary>
        string RawValue { get; }

        /// <summary>
        /// Gets the attributes of the tag (for tags that have attributes)
        /// </summary>
        IReadOnlyDictionary<string, string> Attributes { get; }
    }
} 