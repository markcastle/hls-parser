using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HlsParser.Models;

namespace HlsParser.Parsers
{
    /// <summary>
    /// Parser for HLS playlists
    /// </summary>
    public class PlaylistParser
    {
        private readonly IList<ITagParser> _tagParsers;
        private readonly GeneralTagParser _fallbackParser;

        /// <summary>
        /// Creates a new instance of the PlaylistParser class
        /// </summary>
        public PlaylistParser()
        {
            _tagParsers = new List<ITagParser>
            {
                new VersionTagParser(),
                new StreamInfTagParser(),
                new InfoTagParser(),
                // Add more specific tag parsers here
            };

            _fallbackParser = new GeneralTagParser();
        }

        /// <summary>
        /// Parses the specified content as an HLS playlist
        /// </summary>
        /// <param name="content">The playlist content</param>
        /// <param name="playlistUri">The URI of the playlist</param>
        /// <returns>The parsed playlist</returns>
        public Playlist Parse(string content, Uri playlistUri)
        {
            if (string.IsNullOrEmpty(content))
                throw new ArgumentException("Content cannot be null or empty.", nameof(content));

            if (playlistUri == null)
                throw new ArgumentNullException(nameof(playlistUri));

            using (var reader = new StringReader(content))
            {
                string line = reader.ReadLine();
                if (line == null || !line.StartsWith("#EXTM3U"))
                    throw new FormatException("Invalid playlist format. Missing #EXTM3U tag.");

                var tags = ParseTags(reader);
                var version = ExtractVersion(tags);

                if (IsMasterPlaylist(tags))
                {
                    return ParseMasterPlaylist(tags, version, playlistUri);
                }
                else
                {
                    return ParseMediaPlaylist(tags, version, playlistUri);
                }
            }
        }

        private IList<Tag> ParseTags(StringReader reader)
        {
            var tags = new List<Tag>();
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("#"))
                {
                    var tag = ParseTag(line);
                    if (tag != null)
                    {
                        tags.Add(tag);
                    }
                }
                else
                {
                    // This is a URI line, associate it with the last tag
                    if (tags.Count > 0)
                    {
                        var lastTag = tags[tags.Count - 1];
                        lastTag.Attributes["URI"] = line;
                    }
                }
            }

            return tags;
        }

        private Tag ParseTag(string line)
        {
            if (string.IsNullOrEmpty(line) || !line.StartsWith("#"))
                return null;

            string tagName = line.Substring(1).Split(new[] { ':' }, 2)[0];

            foreach (var parser in _tagParsers)
            {
                if (parser.CanParse(tagName))
                {
                    var tag = parser.Parse(line);
                    if (tag != null)
                        return tag;
                }
            }

            // Use the fallback parser if no specific parser is found
            return _fallbackParser.Parse(line);
        }

        private int ExtractVersion(IList<Tag> tags)
        {
            var versionTag = tags.FirstOrDefault(t => t.Name == "EXT-X-VERSION");
            if (versionTag != null && !string.IsNullOrEmpty(versionTag.RawValue))
            {
                if (int.TryParse(versionTag.RawValue, out int version))
                {
                    return version;
                }
            }

            return 1; // Default version is 1
        }

        private bool IsMasterPlaylist(IList<Tag> tags)
        {
            // Master playlists contain EXT-X-STREAM-INF tags
            return tags.Any(t => t.Name == "EXT-X-STREAM-INF");
        }

        private MasterPlaylist ParseMasterPlaylist(IList<Tag> tags, int version, Uri playlistUri)
        {
            var playlist = new MasterPlaylist
            {
                Version = version,
                Uri = playlistUri,
                Tags = tags
            };

            // Parse stream infos
            foreach (var tag in tags.Where(t => t.Name == "EXT-X-STREAM-INF"))
            {
                var stream = new StreamInfo();

                if (tag.Attributes.TryGetValue("URI", out var uriStr))
                {
                    var uri = ResolveUri(playlistUri, uriStr);
                    stream.Uri = uri;
                    tag.Attributes.Remove("URI");
                }

                if (tag.Attributes.TryGetValue("BANDWIDTH", out var bandwidth) && 
                    int.TryParse(bandwidth, out var bandwidthValue))
                {
                    stream.Bandwidth = bandwidthValue;
                }

                if (tag.Attributes.TryGetValue("AVERAGE-BANDWIDTH", out var avgBandwidth) && 
                    int.TryParse(avgBandwidth, out var avgBandwidthValue))
                {
                    stream.AverageBandwidth = avgBandwidthValue;
                }

                if (tag.Attributes.TryGetValue("CODECS", out var codecs))
                {
                    stream.Codecs = codecs;
                }

                if (tag.Attributes.TryGetValue("RESOLUTION", out var resolution))
                {
                    stream.Resolution = resolution;
                }

                if (tag.Attributes.TryGetValue("FRAME-RATE", out var frameRate) && 
                    float.TryParse(frameRate, out var frameRateValue))
                {
                    stream.FrameRate = frameRateValue;
                }

                if (tag.Attributes.TryGetValue("NAME", out var name))
                {
                    stream.Name = name;
                }

                if (tag.Attributes.TryGetValue("AUDIO", out var audioGroup))
                {
                    stream.AudioGroupId = audioGroup;
                }

                if (tag.Attributes.TryGetValue("SUBTITLES", out var subtitlesGroup))
                {
                    stream.SubtitleGroupId = subtitlesGroup;
                }

                if (tag.Attributes.TryGetValue("CLOSED-CAPTIONS", out var ccGroup))
                {
                    stream.ClosedCaptionsGroupId = ccGroup;
                }

                if (tag.Attributes.TryGetValue("VIDEO-RANGE", out var videoRange))
                {
                    stream.VideoRange = videoRange;
                }

                // Copy any remaining attributes
                foreach (var attr in tag.Attributes)
                {
                    if (!stream.Attributes.ContainsKey(attr.Key))
                    {
                        stream.Attributes[attr.Key] = attr.Value;
                    }
                }

                playlist.Streams.Add(stream);
            }

            // Parse rendition groups
            ParseRenditionGroups(tags, playlist, playlistUri);

            return playlist;
        }

        private void ParseRenditionGroups(IList<Tag> tags, MasterPlaylist playlist, Uri playlistUri)
        {
            var renditionTags = tags.Where(t => t.Name == "EXT-X-MEDIA").ToList();
            var groupTypes = renditionTags
                .Where(t => t.Attributes.ContainsKey("TYPE") && t.Attributes.ContainsKey("GROUP-ID"))
                .Select(t => t.Attributes["TYPE"])
                .Distinct()
                .ToList();

            foreach (var type in groupTypes)
            {
                var groupIds = renditionTags
                    .Where(t => t.Attributes["TYPE"] == type)
                    .Select(t => t.Attributes["GROUP-ID"])
                    .Distinct()
                    .ToList();

                foreach (var groupId in groupIds)
                {
                    var group = new RenditionGroup
                    {
                        Type = type,
                        GroupId = groupId
                    };

                    var renditionTagsForGroup = renditionTags
                        .Where(t => t.Attributes["TYPE"] == type && t.Attributes["GROUP-ID"] == groupId)
                        .ToList();

                    foreach (var tag in renditionTagsForGroup)
                    {
                        var rendition = new Rendition();

                        if (tag.Attributes.TryGetValue("URI", out var uriStr))
                        {
                            rendition.Uri = uriStr;
                        }

                        if (tag.Attributes.TryGetValue("NAME", out var name))
                        {
                            rendition.Name = name;
                        }

                        if (tag.Attributes.TryGetValue("LANGUAGE", out var language))
                        {
                            rendition.Language = language;
                        }

                        if (tag.Attributes.TryGetValue("DEFAULT", out var defaultValue))
                        {
                            rendition.Default = defaultValue.Equals("YES", StringComparison.OrdinalIgnoreCase);
                        }

                        if (tag.Attributes.TryGetValue("FORCED", out var forcedValue))
                        {
                            rendition.Forced = forcedValue.Equals("YES", StringComparison.OrdinalIgnoreCase);
                        }

                        if (tag.Attributes.TryGetValue("AUTOSELECT", out var autoselectValue))
                        {
                            rendition.Autoselect = autoselectValue.Equals("YES", StringComparison.OrdinalIgnoreCase);
                        }

                        if (tag.Attributes.TryGetValue("CHARACTERISTICS", out var characteristics))
                        {
                            rendition.Characteristics = characteristics;
                        }

                        // Copy any remaining attributes
                        foreach (var attr in tag.Attributes)
                        {
                            if (!rendition.Attributes.ContainsKey(attr.Key))
                            {
                                rendition.Attributes[attr.Key] = attr.Value;
                            }
                        }

                        group.Renditions.Add(rendition);
                    }

                    playlist.RenditionGroups.Add(group);
                }
            }
        }

        private MediaPlaylist ParseMediaPlaylist(IList<Tag> tags, int version, Uri playlistUri)
        {
            var playlist = new MediaPlaylist
            {
                Version = version,
                Uri = playlistUri,
                Tags = tags,
                IsEndless = !tags.Any(t => t.Name == "EXT-X-ENDLIST")
            };

            // Extract target duration
            var targetDurationTag = tags.FirstOrDefault(t => t.Name == "EXT-X-TARGETDURATION");
            if (targetDurationTag != null && !string.IsNullOrEmpty(targetDurationTag.RawValue))
            {
                if (double.TryParse(targetDurationTag.RawValue, out double targetDuration))
                {
                    playlist.TargetDuration = targetDuration;
                }
            }

            // Extract media sequence number
            var mediaSequenceTag = tags.FirstOrDefault(t => t.Name == "EXT-X-MEDIA-SEQUENCE");
            if (mediaSequenceTag != null && !string.IsNullOrEmpty(mediaSequenceTag.RawValue))
            {
                if (int.TryParse(mediaSequenceTag.RawValue, out int mediaSequence))
                {
                    playlist.MediaSequence = mediaSequence;
                }
            }

            // Extract playlist type
            var playlistTypeTag = tags.FirstOrDefault(t => t.Name == "EXT-X-PLAYLIST-TYPE");
            if (playlistTypeTag != null && !string.IsNullOrEmpty(playlistTypeTag.RawValue))
            {
                playlist.PlaylistType = playlistTypeTag.RawValue;
            }

            // Extract I-frame playlist URI
            var iframeTag = tags.FirstOrDefault(t => t.Name == "EXT-X-I-FRAMES-ONLY");
            if (iframeTag != null)
            {
                playlist.FramePlaylistUri = iframeTag.RawValue;
            }

            // Check if the playlist has discontinuity
            playlist.HasDiscontinuity = tags.Any(t => t.Name == "EXT-X-DISCONTINUITY");

            // Parse segments
            ParseMediaSegments(tags, playlist, playlistUri);

            return playlist;
        }

        private void ParseMediaSegments(IList<Tag> tags, MediaPlaylist playlist, Uri playlistUri)
        {
            var infTags = tags.Where(t => t.Name == "EXTINF").ToList();
            int segmentIndex = 0;
            EncryptionInfo currentEncryption = null;
            bool hasDiscontinuity = false;
            DateTimeOffset? currentProgramDateTime = null;

            foreach (var infTag in infTags)
            {
                if (!infTag.Attributes.TryGetValue("URI", out var uriStr))
                    continue;

                var segment = new MediaSegment
                {
                    Uri = ResolveUri(playlistUri, uriStr),
                    SequenceNumber = playlist.MediaSequence + segmentIndex,
                    HasDiscontinuity = hasDiscontinuity,
                    Encryption = currentEncryption,
                    ProgramDateTime = currentProgramDateTime
                };

                // Parse duration
                if (infTag.Attributes.TryGetValue("DURATION", out var durationStr) &&
                    double.TryParse(durationStr, out var duration))
                {
                    segment.Duration = duration;
                }

                // Parse title
                if (infTag.Attributes.TryGetValue("TITLE", out var title))
                {
                    segment.Title = title;
                }

                // Check for tags between this EXTINF and the next one
                int currentIndex = tags.IndexOf(infTag);
                int nextInfIndex = infTags.Count > segmentIndex + 1
                    ? tags.IndexOf(infTags[segmentIndex + 1])
                    : tags.Count;

                for (int i = currentIndex + 1; i < nextInfIndex; i++)
                {
                    var tag = tags[i];

                    // Skip the URI line
                    if (tag.Name == "URI")
                        continue;

                    // Check for discontinuity
                    if (tag.Name == "EXT-X-DISCONTINUITY")
                    {
                        hasDiscontinuity = true;
                        segment.HasDiscontinuity = true;
                    }

                    // Check for encryption
                    if (tag.Name == "EXT-X-KEY")
                    {
                        currentEncryption = ParseEncryptionInfo(tag, playlistUri);
                        segment.Encryption = currentEncryption;
                    }

                    // Check for byte range
                    if (tag.Name == "EXT-X-BYTERANGE")
                    {
                        segment.ByteRange = tag.RawValue;
                    }

                    // Check for program date time
                    if (tag.Name == "EXT-X-PROGRAM-DATE-TIME")
                    {
                        if (DateTimeOffset.TryParse(tag.RawValue, out var programDateTime))
                        {
                            currentProgramDateTime = programDateTime;
                            segment.ProgramDateTime = programDateTime;
                        }
                    }

                    // Add tag to segment
                    segment.Tags.Add(tag);
                }

                playlist.Segments.Add(segment);
                segmentIndex++;
                hasDiscontinuity = false; // Reset discontinuity flag for next segment
            }
        }

        private EncryptionInfo ParseEncryptionInfo(Tag tag, Uri playlistUri)
        {
            var encryptionInfo = new EncryptionInfo();

            if (tag.Attributes.TryGetValue("METHOD", out var method))
            {
                encryptionInfo.Method = method;
            }

            if (tag.Attributes.TryGetValue("URI", out var uriStr))
            {
                encryptionInfo.KeyUri = ResolveUri(playlistUri, uriStr);
            }

            if (tag.Attributes.TryGetValue("IV", out var iv))
            {
                encryptionInfo.Iv = iv;
            }

            if (tag.Attributes.TryGetValue("KEYFORMAT", out var keyFormat))
            {
                encryptionInfo.KeyFormat = keyFormat;
            }

            if (tag.Attributes.TryGetValue("KEYFORMATVERSIONS", out var keyFormatVersions))
            {
                encryptionInfo.KeyFormatVersions = keyFormatVersions;
            }

            return encryptionInfo;
        }

        private Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if (string.IsNullOrEmpty(relativeUri))
                return null;

            if (Uri.TryCreate(relativeUri, UriKind.Absolute, out var absoluteUri))
                return absoluteUri;

            return new Uri(baseUri, relativeUri);
        }
    }
} 