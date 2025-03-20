using System;
using System.Collections.Generic;

namespace HlsParser.Models
{
    /// <summary>
    /// Represents information about a stream variant in a master playlist
    /// </summary>
    public class StreamInfo
    {
        /// <summary>
        /// The URI of the stream's media playlist
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// The bandwidth in bits per second
        /// </summary>
        public int Bandwidth { get; set; }

        /// <summary>
        /// The average bandwidth in bits per second (if provided)
        /// </summary>
        public int? AverageBandwidth { get; set; }

        /// <summary>
        /// The codec string
        /// </summary>
        public string Codecs { get; set; }

        /// <summary>
        /// The resolution in the format "widthxheight"
        /// </summary>
        public string Resolution { get; set; }

        /// <summary>
        /// The frame rate (if provided)
        /// </summary>
        public float? FrameRate { get; set; }

        /// <summary>
        /// The stream name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The audio group ID that this stream uses
        /// </summary>
        public string AudioGroupId { get; set; }
        
        /// <summary>
        /// The subtitle group ID that this stream uses
        /// </summary>
        public string SubtitleGroupId { get; set; }
        
        /// <summary>
        /// The closed captions group ID that this stream uses
        /// </summary>
        public string ClosedCaptionsGroupId { get; set; }

        /// <summary>
        /// The video range (SDR or HDR)
        /// </summary>
        public string VideoRange { get; set; }

        /// <summary>
        /// Additional attributes
        /// </summary>
        public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Returns a string representation of the stream info
        /// </summary>
        public override string ToString()
        {
            return $"{Resolution} {Bandwidth/1000}kbps {Codecs}";
        }
    }
} 