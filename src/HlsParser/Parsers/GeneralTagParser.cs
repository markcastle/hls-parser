using System.Collections.Generic;
using HlsParser.Models;

namespace HlsParser.Parsers
{
    /// <summary>
    /// General parser for HLS tags
    /// </summary>
    public class GeneralTagParser : BaseTagParser
    {
        private static readonly HashSet<string> AttributeBasedTags = new HashSet<string>
        {
            "EXT-X-MEDIA",
            "EXT-X-I-FRAME-STREAM-INF",
            "EXT-X-SESSION-KEY",
            "EXT-X-SESSION-DATA",
            "EXT-X-KEY",
            "EXT-X-MAP",
            "EXT-X-DATERANGE"
        };

        /// <summary>
        /// Gets the tag name that this parser can parse
        /// </summary>
        protected override string TagName => string.Empty;

        /// <summary>
        /// Determines if this parser can parse the specified tag
        /// </summary>
        /// <param name="tagName">The tag name without the # prefix</param>
        /// <returns>True if this parser can parse the tag; otherwise, false</returns>
        public override bool CanParse(string tagName)
        {
            // This is a fallback parser that can parse any tag
            return true;
        }

        /// <summary>
        /// Parses the specified line as a tag
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <returns>The parsed tag</returns>
        public override Tag Parse(string line)
        {
            if (TryExtractTagNameAndValue(line, out var tagName, out var rawValue))
            {
                var tag = new Tag(tagName, rawValue);

                if (AttributeBasedTags.Contains(tagName))
                {
                    ExtractAttributes(rawValue, tag);
                }

                return tag;
            }

            return null;
        }
    }
} 