using System.Collections.Generic;

namespace HlsParser.Models
{
    /// <summary>
    /// Represents an HLS tag
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// The name of the tag without the # prefix (e.g., EXT-X-VERSION)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The raw value of the tag (everything after the colon)
        /// </summary>
        public string RawValue { get; set; }

        /// <summary>
        /// Attributes parsed from the tag value if available
        /// </summary>
        public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Creates a new instance of the Tag class
        /// </summary>
        public Tag() { }

        /// <summary>
        /// Creates a new instance of the Tag class with the specified name
        /// </summary>
        /// <param name="name">The tag name without the # prefix</param>
        public Tag(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Creates a new instance of the Tag class with the specified name and value
        /// </summary>
        /// <param name="name">The tag name without the # prefix</param>
        /// <param name="value">The raw tag value</param>
        public Tag(string name, string value)
        {
            Name = name;
            RawValue = value;
        }

        /// <summary>
        /// Returns the tag name
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
} 