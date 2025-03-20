using System;
using HlsParser.Parsers;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Adapter for the HlsParser class that implements the IHlsParser interface
    /// </summary>
    internal class HlsParserAdapter : IHlsParser
    {
        private readonly PlaylistParser _parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="HlsParserAdapter"/> class
        /// </summary>
        public HlsParserAdapter()
        {
            _parser = new PlaylistParser();
        }

        /// <inheritdoc/>
        public IPlaylist Parse(string content, Uri uri)
        {
            var playlist = _parser.Parse(content, uri);
            
            if (playlist is Models.MasterPlaylist masterPlaylist)
            {
                return new MasterPlaylistAdapter(masterPlaylist);
            }
            else if (playlist is Models.MediaPlaylist mediaPlaylist)
            {
                return new MediaPlaylistAdapter(mediaPlaylist);
            }
            
            throw new InvalidOperationException("Unknown playlist type");
        }

        /// <inheritdoc/>
        public IMasterPlaylist ParseMasterPlaylist(string content, Uri uri)
        {
            // Use Parse and then check if it's a master playlist
            var playlist = _parser.Parse(content, uri);
            
            if (playlist is Models.MasterPlaylist masterPlaylist)
            {
                return new MasterPlaylistAdapter(masterPlaylist);
            }
            
            throw new InvalidOperationException("Content is not a master playlist");
        }

        /// <inheritdoc/>
        public IMediaPlaylist ParseMediaPlaylist(string content, Uri uri)
        {
            // Use Parse and then check if it's a media playlist
            var playlist = _parser.Parse(content, uri);
            
            if (playlist is Models.MediaPlaylist mediaPlaylist)
            {
                return new MediaPlaylistAdapter(mediaPlaylist);
            }
            
            throw new InvalidOperationException("Content is not a media playlist");
        }
    }
} 