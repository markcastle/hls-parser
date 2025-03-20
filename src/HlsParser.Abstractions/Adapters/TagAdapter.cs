using System;
using System.Collections.Generic;
using HlsParser.Models;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Adapter for the Tag class that implements the ITag interface
    /// </summary>
    internal class TagAdapter : ITag
    {
        private readonly Tag _tag;
        private readonly IReadOnlyDictionary<string, string> _attributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagAdapter"/> class
        /// </summary>
        /// <param name="tag">The tag to adapt</param>
        public TagAdapter(Tag tag)
        {
            _tag = tag ?? throw new ArgumentNullException(nameof(tag));
            
            // Convert IDictionary to IReadOnlyDictionary
            if (tag.Attributes != null)
            {
                var dict = new Dictionary<string, string>();
                foreach (var kvp in tag.Attributes)
                {
                    dict.Add(kvp.Key, kvp.Value);
                }
                _attributes = dict;
            }
            else
            {
                _attributes = new Dictionary<string, string>();
            }
        }

        /// <inheritdoc/>
        public string Name => _tag.Name;

        /// <inheritdoc/>
        public string RawValue => _tag.RawValue;

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, string> Attributes => _attributes;
    }
} 