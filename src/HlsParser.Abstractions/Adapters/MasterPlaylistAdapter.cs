using System;
using System.Collections.Generic;
using System.Linq;
using HlsParser.Models;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Adapter for the MasterPlaylist class that implements the IMasterPlaylist interface
    /// </summary>
    internal class MasterPlaylistAdapter : IMasterPlaylist
    {
        private readonly MasterPlaylist _playlist;
        private readonly List<IStreamInfo> _streams;
        private readonly List<IRenditionGroup> _renditionGroups;
        private readonly List<ITag> _tags;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPlaylistAdapter"/> class
        /// </summary>
        /// <param name="playlist">The master playlist to adapt</param>
        public MasterPlaylistAdapter(MasterPlaylist playlist)
        {
            _playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
            
            _streams = playlist.Streams?.Select(s => new StreamInfoAdapter(s)).Cast<IStreamInfo>().ToList() 
                ?? new List<IStreamInfo>();
                
            _renditionGroups = playlist.RenditionGroups?.Select(g => new RenditionGroupAdapter(g)).Cast<IRenditionGroup>().ToList() 
                ?? new List<IRenditionGroup>();
                
            _tags = playlist.Tags?.Select(t => new TagAdapter(t)).Cast<ITag>().ToList() 
                ?? new List<ITag>();
        }

        /// <inheritdoc/>
        public int Version => _playlist.Version;

        /// <inheritdoc/>
        public Uri Uri => _playlist.Uri;

        /// <inheritdoc/>
        public IReadOnlyList<ITag> Tags => _tags;

        /// <inheritdoc/>
        public IReadOnlyList<IStreamInfo> Streams => _streams;

        /// <inheritdoc/>
        public IReadOnlyList<IRenditionGroup> RenditionGroups => _renditionGroups;
    }
} 