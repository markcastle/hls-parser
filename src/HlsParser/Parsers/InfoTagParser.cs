using System;
using System.Text.RegularExpressions;
using HlsParser.Models;

namespace HlsParser.Parsers
{
    /// <summary>
    /// Parser for the EXTINF tag
    /// </summary>
    public class InfoTagParser : BaseTagParser
    {
        private static readonly Regex DurationPattern = new Regex(@"^([\d.]+)(?:,(.*))?$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the tag name that this parser can parse
        /// </summary>
        protected override string TagName => "EXTINF";

        /// <summary>
        /// Parses the specified line as a tag
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <returns>The parsed tag</returns>
        public override Tag Parse(string line)
        {
            if (TryExtractTagNameAndValue(line, out var tagName, out var rawValue) && tagName == TagName)
            {
                var tag = new Tag(tagName, rawValue);

                var match = DurationPattern.Match(rawValue);
                if (match.Success)
                {
                    string durationStr = match.Groups[1].Value;
                    string title = match.Groups[2].Success ? match.Groups[2].Value : null;

                    if (double.TryParse(durationStr, out var duration))
                    {
                        tag.Attributes["DURATION"] = duration.ToString();
                    }

                    if (!string.IsNullOrEmpty(title))
                    {
                        tag.Attributes["TITLE"] = title;
                    }
                }

                return tag;
            }

            return null;
        }
    }
} 