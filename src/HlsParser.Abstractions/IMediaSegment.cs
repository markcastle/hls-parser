using System;
using System.Collections.Generic;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Interface for a media segment in a media playlist
    /// </summary>
    public interface IMediaSegment
    {
        /// <summary>
        /// Gets the URI of the segment
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Gets the duration of the segment in seconds
        /// </summary>
        double Duration { get; }

        /// <summary>
        /// Gets the title of the segment if provided
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the sequence number of the segment
        /// </summary>
        int SequenceNumber { get; }

        /// <summary>
        /// Gets a value indicating whether this segment has a discontinuity from the previous one
        /// </summary>
        bool HasDiscontinuity { get; }

        /// <summary>
        /// Gets the byte range if the segment is a partial file
        /// </summary>
        string ByteRange { get; }

        /// <summary>
        /// Gets the encryption information for this segment
        /// </summary>
        IEncryptionInfo? Encryption { get; }

        /// <summary>
        /// Gets the date and time corresponding to the segment if provided
        /// </summary>
        DateTimeOffset? ProgramDateTime { get; }

        /// <summary>
        /// Gets the tags associated with this segment
        /// </summary>
        IReadOnlyList<ITag> Tags { get; }
    }
} 