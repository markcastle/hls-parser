# 📺 HLS Parser

A robust HTTP Live Streaming (HLS) parser library for .NET applications. This library targets .NET Standard 2.1 for wide compatibility.

## ✨ Features

- 🎬 Parse master and media playlists
- 🏷️ Support for EXT-X tags specified in the HLS specification
- 🔄 Stream variant handling
- 🔒 Encryption support (AES-128, SAMPLE-AES)
- 📥 Segment download
- 🧩 Clean API for client applications

## 🚀 Getting Started

### 📦 Installation

```
dotnet add package HlsParser
```

### 💻 Basic Usage

```csharp
using System;
using System.Threading.Tasks;
using HlsParser;
using HlsParser.Models;

// Initialize the client
using var client = new HlsClient();

// Get a master playlist
var masterPlaylist = await client.GetMasterPlaylistAsync(new Uri("https://example.com/master.m3u8"));
Console.WriteLine($"Found {masterPlaylist.Streams.Count} stream variants");

// Get the first stream variant
if (masterPlaylist.Streams.Count > 0)
{
    var stream = masterPlaylist.Streams[0];
    Console.WriteLine($"Selected stream: {stream.Resolution} {stream.Bandwidth/1000}kbps {stream.Codecs}");
    
    // Get the media playlist for this stream
    var mediaPlaylist = await client.GetMediaPlaylistAsync(stream.Uri);
    Console.WriteLine($"Found {mediaPlaylist.Segments.Count} segments");
    
    // Download a segment
    if (mediaPlaylist.Segments.Count > 0)
    {
        var segment = mediaPlaylist.Segments[0];
        var segmentData = await client.GetSegmentAsync(segment);
        Console.WriteLine($"Downloaded segment: {segmentData.Length} bytes");
    }
}
```

## 📚 API Reference

### 🧰 HlsClient

The main client for downloading and parsing HLS playlists.

```csharp
// Create a client
var client = new HlsClient();

// Get a master playlist
var masterPlaylist = await client.GetMasterPlaylistAsync(uri);

// Get a media playlist
var mediaPlaylist = await client.GetMediaPlaylistAsync(uri);

// Get a media segment
var segmentData = await client.GetSegmentAsync(segment);
```

### 📋 Models

#### 📄 Playlist

Base class for HLS playlists.

```csharp
public abstract class Playlist
{
    public int Version { get; set; }
    public Uri Uri { get; set; }
    public IList<Tag> Tags { get; set; }
}
```

#### 📑 MasterPlaylist

Represents an HLS master playlist containing stream variants.

```csharp
public class MasterPlaylist : Playlist
{
    public IList<StreamInfo> Streams { get; set; }
    public IList<RenditionGroup> RenditionGroups { get; set; }
}
```

#### 📝 MediaPlaylist

Represents an HLS media playlist containing media segments.

```csharp
public class MediaPlaylist : Playlist
{
    public double TargetDuration { get; set; }
    public bool IsEndless { get; set; }
    public int MediaSequence { get; set; }
    public bool HasDiscontinuity { get; set; }
    public string PlaylistType { get; set; }
    public IList<MediaSegment> Segments { get; set; }
    public string IFramePlaylistUri { get; set; }
}
```

#### 🎞️ MediaSegment

Represents a media segment in a media playlist.

```csharp
public class MediaSegment
{
    public Uri Uri { get; set; }
    public double Duration { get; set; }
    public string Title { get; set; }
    public int SequenceNumber { get; set; }
    public bool HasDiscontinuity { get; set; }
    public string ByteRange { get; set; }
    public EncryptionInfo Encryption { get; set; }
    public DateTimeOffset? ProgramDateTime { get; set; }
    public IList<Tag> Tags { get; set; }
}
```

## 📜 License

MIT 