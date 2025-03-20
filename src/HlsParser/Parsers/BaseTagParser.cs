using System.Text.RegularExpressions;
using HlsParser.Models;

namespace HlsParser.Parsers
{
    /// <summary>
    /// Base implementation for tag parsers
    /// </summary>
    public abstract class BaseTagParser : ITagParser
    {
        /// <summary>
        /// The tag name pattern
        /// </summary>
        protected static readonly Regex TagPattern = new Regex(@"^#([^:]+)(?::(.*))?$", RegexOptions.Compiled);

        /// <summary>
        /// The key-value attribute pattern for tag values
        /// </summary>
        protected static readonly Regex AttributePattern = new Regex(@"([A-Z0-9-]+)=(?:""([^""]*)""|([^,""]*))(?:,|$)", RegexOptions.Compiled);

        /// <summary>
        /// Gets the tag name that this parser can parse
        /// </summary>
        protected abstract string TagName { get; }

        /// <summary>
        /// Determines if this parser can parse the specified tag
        /// </summary>
        /// <param name="tagName">The tag name without the # prefix</param>
        /// <returns>True if this parser can parse the tag; otherwise, false</returns>
        public virtual bool CanParse(string tagName)
        {
            return tagName.Equals(TagName);
        }

        /// <summary>
        /// Parses the specified line as a tag
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <returns>The parsed tag</returns>
        public abstract Tag Parse(string line);

        /// <summary>
        /// Extracts the tag name and raw value from a line
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <param name="tagName">The extracted tag name</param>
        /// <param name="rawValue">The extracted raw value</param>
        /// <returns>True if the line is a valid tag; otherwise, false</returns>
        protected bool TryExtractTagNameAndValue(string line, out string tagName, out string rawValue)
        {
            var match = TagPattern.Match(line);
            if (match.Success)
            {
                tagName = match.Groups[1].Value;
                rawValue = match.Groups[2].Success ? match.Groups[2].Value : null;
                return true;
            }

            tagName = null;
            rawValue = null;
            return false;
        }

        /// <summary>
        /// Extracts attributes from a tag value
        /// </summary>
        /// <param name="value">The tag value</param>
        /// <param name="tag">The tag to populate with attributes</param>
        protected void ExtractAttributes(string value, Tag tag)
        {
            if (string.IsNullOrEmpty(value))
                return;

            var matches = AttributePattern.Matches(value);
            foreach (Match match in matches)
            {
                string key = match.Groups[1].Value;
                string val = match.Groups[2].Success ? match.Groups[2].Value : match.Groups[3].Value;
                tag.Attributes[key] = val;
            }
        }
    }
} 