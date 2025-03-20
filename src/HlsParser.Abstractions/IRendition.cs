using System;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Interface for a rendition in a rendition group
    /// </summary>
    public interface IRendition
    {
        /// <summary>
        /// Gets the URI of the rendition's media playlist
        /// </summary>
        Uri? Uri { get; }

        /// <summary>
        /// Gets the display name of this rendition
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the language of this rendition if provided
        /// </summary>
        string Language { get; }

        /// <summary>
        /// Gets a value indicating whether this rendition is the default
        /// </summary>
        bool IsDefault { get; }

        /// <summary>
        /// Gets a value indicating whether this rendition should be auto-selected by the player
        /// </summary>
        bool Autoselect { get; }

        /// <summary>
        /// Gets a value indicating whether this rendition is forced
        /// </summary>
        bool Forced { get; }

        /// <summary>
        /// Gets the assoc-language attribute if provided
        /// </summary>
        string AssocLanguage { get; }

        /// <summary>
        /// Gets the channels attribute if provided
        /// </summary>
        string Channels { get; }

        /// <summary>
        /// Gets the characteristics attribute if provided
        /// </summary>
        string Characteristics { get; }
    }
} 