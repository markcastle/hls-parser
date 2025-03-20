using System;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Interface for a stream variant in a master playlist
    /// </summary>
    public interface IStreamInfo
    {
        /// <summary>
        /// Gets the URI of the stream's media playlist
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Gets the bandwidth of the stream in bits per second
        /// </summary>
        int Bandwidth { get; }

        /// <summary>
        /// Gets the average bandwidth of the stream in bits per second if provided
        /// </summary>
        int? AverageBandwidth { get; }

        /// <summary>
        /// Gets the codecs used by this stream (comma-separated string)
        /// </summary>
        string Codecs { get; }

        /// <summary>
        /// Gets the resolution of this stream (e.g., "1920x1080")
        /// </summary>
        string Resolution { get; }

        /// <summary>
        /// Gets the frame rate of this stream if provided
        /// </summary>
        float? FrameRate { get; }

        /// <summary>
        /// Gets the display name of this stream if provided
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the audio group ID for this stream if it uses external audio
        /// </summary>
        string AudioGroupId { get; }

        /// <summary>
        /// Gets the subtitle group ID for this stream if it uses external subtitles
        /// </summary>
        string SubtitleGroupId { get; }

        /// <summary>
        /// Gets the closed captions group ID for this stream if it uses external closed captions
        /// </summary>
        string ClosedCaptionsGroupId { get; }

        /// <summary>
        /// Gets the video range type for this stream (SDR, HLG, PQ)
        /// </summary>
        string VideoRange { get; }
    }
} 