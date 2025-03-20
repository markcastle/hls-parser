using System;
using System.Collections.Generic;
using System.Linq;
using HlsParser.Models;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Adapter for the MediaPlaylist class that implements the IMediaPlaylist interface
    /// </summary>
    internal class MediaPlaylistAdapter : IMediaPlaylist
    {
        private readonly MediaPlaylist _playlist;
        private readonly List<IMediaSegment> _segments;
        private readonly List<ITag> _tags;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlaylistAdapter"/> class
        /// </summary>
        /// <param name="playlist">The media playlist to adapt</param>
        public MediaPlaylistAdapter(MediaPlaylist playlist)
        {
            _playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
            
            _segments = playlist.Segments?.Select(s => new MediaSegmentAdapter(s)).Cast<IMediaSegment>().ToList() 
                ?? new List<IMediaSegment>();
                
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
        public double TargetDuration => _playlist.TargetDuration;

        /// <inheritdoc/>
        public bool IsEndless => _playlist.IsEndless;

        /// <inheritdoc/>
        public int MediaSequence => _playlist.MediaSequence;

        /// <inheritdoc/>
        public bool HasDiscontinuity => _playlist.HasDiscontinuity;

        /// <inheritdoc/>
        public string PlaylistType => _playlist.PlaylistType;

        /// <inheritdoc/>
        public IReadOnlyList<IMediaSegment> Segments => _segments;
        
        /// <inheritdoc/>
        public string IFramePlaylistUri => string.Empty; // Not available in original model, return empty string
    }
} 