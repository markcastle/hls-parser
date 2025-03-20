using System;
using System.Collections.Generic;
using System.Linq;
using HlsParser.Models;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Adapter for the RenditionGroup class that implements the IRenditionGroup interface
    /// </summary>
    internal class RenditionGroupAdapter : IRenditionGroup
    {
        private readonly RenditionGroup _group;
        private readonly List<IRendition> _renditions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenditionGroupAdapter"/> class
        /// </summary>
        /// <param name="group">The rendition group to adapt</param>
        public RenditionGroupAdapter(RenditionGroup group)
        {
            _group = group ?? throw new ArgumentNullException(nameof(group));
            
            _renditions = group.Renditions?.Select(r => new RenditionAdapter(r)).Cast<IRendition>().ToList() 
                ?? new List<IRendition>();
        }

        /// <inheritdoc/>
        public string Type => _group.Type;

        /// <inheritdoc/>
        public string GroupId => _group.GroupId;

        /// <inheritdoc/>
        public IReadOnlyList<IRendition> Renditions => _renditions;
    }

    /// <summary>
    /// Adapter for the Rendition class that implements the IRendition interface
    /// </summary>
    internal class RenditionAdapter : IRendition
    {
        private readonly Rendition _rendition;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenditionAdapter"/> class
        /// </summary>
        /// <param name="rendition">The rendition to adapt</param>
        public RenditionAdapter(Rendition rendition)
        {
            _rendition = rendition ?? throw new ArgumentNullException(nameof(rendition));
        }

        /// <inheritdoc/>
        public Uri Uri
        {
            get
            {
                if (string.IsNullOrEmpty(_rendition.Uri))
                    return null!;
                
                try
                {
                    return new Uri(_rendition.Uri);
                }
                catch (UriFormatException)
                {
                    // Return null for invalid URIs
                    return null!;
                }
            }
        }

        /// <inheritdoc/>
        public string Name => _rendition.Name;

        /// <inheritdoc/>
        public string Language => _rendition.Language;

        /// <inheritdoc/>
        public bool IsDefault => false;

        /// <inheritdoc/>
        public bool Autoselect => false;

        /// <inheritdoc/>
        public bool Forced => false;

        /// <inheritdoc/>
        public string AssocLanguage => string.Empty;

        /// <inheritdoc/>
        public string Channels => string.Empty;

        /// <inheritdoc/>
        public string Characteristics => _rendition.Characteristics;
    }
} 