using System;
using System.Collections.Generic;

namespace HlsParser.Models
{
    /// <summary>
    /// Represents a media segment in a media playlist
    /// </summary>
    public class MediaSegment
    {
        /// <summary>
        /// The URI of the segment
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// The duration of the segment in seconds
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// The title of the segment if provided
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The sequence number of the segment
        /// </summary>
        public int SequenceNumber { get; set; }

        /// <summary>
        /// Indicates if this segment has a discontinuity from the previous one
        /// </summary>
        public bool HasDiscontinuity { get; set; }

        /// <summary>
        /// The byte range if the segment is a partial file
        /// </summary>
        public string ByteRange { get; set; }

        /// <summary>
        /// The encryption information for this segment
        /// </summary>
        public EncryptionInfo Encryption { get; set; }

        /// <summary>
        /// The date and time corresponding to the segment if provided
        /// </summary>
        public DateTimeOffset? ProgramDateTime { get; set; }

        /// <summary>
        /// Additional tags associated with this segment
        /// </summary>
        public IList<Tag> Tags { get; set; } = new List<Tag>();
    }

    /// <summary>
    /// Represents encryption information for a media segment
    /// </summary>
    public class EncryptionInfo
    {
        /// <summary>
        /// The encryption method (AES-128, SAMPLE-AES, etc.)
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The URI of the key file
        /// </summary>
        public Uri KeyUri { get; set; }

        /// <summary>
        /// The initialization vector if provided
        /// </summary>
        public string Iv { get; set; }

        /// <summary>
        /// The key format if provided
        /// </summary>
        public string KeyFormat { get; set; }

        /// <summary>
        /// The key format versions if provided
        /// </summary>
        public string KeyFormatVersions { get; set; }
    }
} 