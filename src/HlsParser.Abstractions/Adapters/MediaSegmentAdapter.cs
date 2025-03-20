using System;
using System.Collections.Generic;
using System.Linq;
using HlsParser.Models;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Adapter for the MediaSegment class that implements the IMediaSegment interface
    /// </summary>
    internal class MediaSegmentAdapter : IMediaSegment
    {
        private readonly MediaSegment _segment;
        private readonly IEncryptionInfo? _encryption;
        private readonly List<ITag> _tags;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaSegmentAdapter"/> class
        /// </summary>
        /// <param name="segment">The media segment to adapt</param>
        public MediaSegmentAdapter(MediaSegment segment)
        {
            _segment = segment ?? throw new ArgumentNullException(nameof(segment));
            
            if (segment.Encryption != null)
            {
                _encryption = new EncryptionInfoAdapter(segment.Encryption);
            }
            
            _tags = segment.Tags?.Select(t => new TagAdapter(t)).Cast<ITag>().ToList() 
                ?? new List<ITag>();
        }

        /// <inheritdoc/>
        public Uri Uri => _segment.Uri;

        /// <inheritdoc/>
        public double Duration => _segment.Duration;

        /// <inheritdoc/>
        public string Title => _segment.Title;

        /// <inheritdoc/>
        public int SequenceNumber => _segment.SequenceNumber;

        /// <inheritdoc/>
        public bool HasDiscontinuity => _segment.HasDiscontinuity;

        /// <inheritdoc/>
        public string ByteRange => _segment.ByteRange;

        /// <inheritdoc/>
        public IEncryptionInfo? Encryption => _encryption;

        /// <inheritdoc/>
        public DateTimeOffset? ProgramDateTime => _segment.ProgramDateTime;

        /// <inheritdoc/>
        public IReadOnlyList<ITag> Tags => _tags;
    }
} 