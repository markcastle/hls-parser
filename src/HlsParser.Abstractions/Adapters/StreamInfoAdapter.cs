using System;
using HlsParser.Models;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Adapter for the StreamInfo class that implements the IStreamInfo interface
    /// </summary>
    internal class StreamInfoAdapter : IStreamInfo
    {
        private readonly StreamInfo _stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamInfoAdapter"/> class
        /// </summary>
        /// <param name="stream">The stream info to adapt</param>
        public StreamInfoAdapter(StreamInfo stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <inheritdoc/>
        public Uri Uri => _stream.Uri;

        /// <inheritdoc/>
        public int Bandwidth => _stream.Bandwidth;

        /// <inheritdoc/>
        public int? AverageBandwidth => _stream.AverageBandwidth;

        /// <inheritdoc/>
        public string Codecs => _stream.Codecs;

        /// <inheritdoc/>
        public string Resolution => _stream.Resolution;

        /// <inheritdoc/>
        public float? FrameRate => _stream.FrameRate;

        /// <inheritdoc/>
        public string Name => _stream.Name;

        /// <inheritdoc/>
        public string AudioGroupId => _stream.AudioGroupId;

        /// <inheritdoc/>
        public string SubtitleGroupId => _stream.SubtitleGroupId;

        /// <inheritdoc/>
        public string ClosedCaptionsGroupId => _stream.ClosedCaptionsGroupId;

        /// <inheritdoc/>
        public string VideoRange => _stream.VideoRange;
    }
} 