using System.Collections.Generic;

namespace HlsParser.Models
{
    /// <summary>
    /// Represents an HLS master playlist containing stream variants
    /// </summary>
    public class MasterPlaylist : Playlist
    {
        /// <summary>
        /// The stream variants available in this master playlist
        /// </summary>
        public IList<StreamInfo> Streams { get; set; } = new List<StreamInfo>();

        /// <summary>
        /// Alternative rendition groups (audio, subtitles, etc.)
        /// </summary>
        public IList<RenditionGroup> RenditionGroups { get; set; } = new List<RenditionGroup>();
    }

    /// <summary>
    /// Represents a group of related renditions (audio, subtitles, etc.)
    /// </summary>
    public class RenditionGroup
    {
        /// <summary>
        /// The type of the rendition group (AUDIO, SUBTITLES, etc.)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The group identifier
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// The renditions in this group
        /// </summary>
        public IList<Rendition> Renditions { get; set; } = new List<Rendition>();
    }

    /// <summary>
    /// Represents a rendition within a rendition group
    /// </summary>
    public class Rendition
    {
        /// <summary>
        /// The URI of the rendition playlist
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// The rendition name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The language of the rendition
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Indicates if this is the default rendition for the group
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// Indicates if this rendition is forced
        /// </summary>
        public bool Forced { get; set; }

        /// <summary>
        /// Indicates if this rendition is autoselect
        /// </summary>
        public bool Autoselect { get; set; }

        /// <summary>
        /// The characteristics of the rendition
        /// </summary>
        public string Characteristics { get; set; }

        /// <summary>
        /// Additional attributes
        /// </summary>
        public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
} 