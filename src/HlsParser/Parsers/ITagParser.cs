using HlsParser.Models;

namespace HlsParser.Parsers
{
    /// <summary>
    /// Interface for tag parsers
    /// </summary>
    public interface ITagParser
    {
        /// <summary>
        /// Determines if this parser can parse the specified tag
        /// </summary>
        /// <param name="tagName">The tag name without the # prefix</param>
        /// <returns>True if this parser can parse the tag; otherwise, false</returns>
        bool CanParse(string tagName);

        /// <summary>
        /// Parses the specified line as a tag
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <returns>The parsed tag</returns>
        Tag Parse(string line);
    }
} 