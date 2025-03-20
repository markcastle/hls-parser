using HlsParser.Models;

namespace HlsParser.Parsers
{
    /// <summary>
    /// Parser for the EXT-X-VERSION tag
    /// </summary>
    public class VersionTagParser : BaseTagParser
    {
        /// <summary>
        /// Gets the tag name that this parser can parse
        /// </summary>
        protected override string TagName => "EXT-X-VERSION";

        /// <summary>
        /// Parses the specified line as a tag
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <returns>The parsed tag</returns>
        public override Tag Parse(string line)
        {
            if (TryExtractTagNameAndValue(line, out var tagName, out var rawValue) && tagName == TagName)
            {
                return new Tag(tagName, rawValue);
            }

            return null;
        }
    }
} 