# 📺 HLS Parser

A robust HTTP Live Streaming (HLS) parser library for .NET applications. This library targets .NET Standard 2.1 for wide compatibility.

## ✨ Features

- 🎬 Parse master and media playlists
- 🏷️ Support for EXT-X tags specified in the HLS specification
- 🔄 Stream variant handling
- 🔒 Encryption metadata parsing (AES-128, SAMPLE-AES) - *Note: Provides encryption information but does not implement decryption*
- 📥 Segment download
- 🧩 Clean API for client applications
- 🖥️ Command line tool for analyzing playlists

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
        
        // If the segment is encrypted, you'll need to implement your own decryption
        if (segment.Encryption != null)
        {
            Console.WriteLine($"Segment is encrypted with method: {segment.Encryption.Method}");
            Console.WriteLine($"Key URI: {segment.Encryption.KeyUri}");
            // Decryption must be implemented by the consumer of this library
        }
    }
}
```

## 🖥️ Command Line Tool

The HLS Parser comes with a command line tool that allows you to analyze HLS playlists directly from the terminal.

### Features

- Analyze both master and media playlists
- Display detailed information about stream variants, renditions, and segments
- Automatically detect playlist type
- Show all playlist tags

### Usage

```
HlsParser <playlist_url>
```

### Examples

**Analyzing a master playlist:**

```
HlsParser https://bitdash-a.akamaihd.net/content/sintel/hls/playlist.m3u8
```

This will display:
- Playlist version and URI
- List of stream variants with resolution, bandwidth, and codec information
- Audio and subtitle rendition groups
- All tags from the playlist

**Analyzing a media playlist:**

```
HlsParser https://bitdash-a.akamaihd.net/content/sintel/hls/video/1500kbit.m3u8
```

This will display:
- Playlist version and URI
- Target duration and media sequence number
- Endless status and discontinuity information
- Detailed information about the first 10 segments
- All tags from the playlist

### Building a Single Executable

To build the command line tool as a single executable with all dependencies embedded:

```
# For Windows
dotnet publish src/HlsParser.CommandLine/HlsParser.CommandLine.csproj -c Release -r win-x64 --self-contained true

# For macOS
dotnet publish src/HlsParser.CommandLine/HlsParser.CommandLine.csproj -c Release -r osx-x64 --self-contained true

# For Linux
dotnet publish src/HlsParser.CommandLine/HlsParser.CommandLine.csproj -c Release -r linux-x64 --self-contained true
```

Note: You may need to edit the `HlsParser.CommandLine.csproj` file to set the correct `RuntimeIdentifier` for your target platform:

```xml
<RuntimeIdentifier>win-x64</RuntimeIdentifier>  <!-- Change to osx-x64 or linux-x64 as needed -->
```

The executable will be created in the `src/HlsParser.CommandLine/bin/Release/net8.0/[platform]/publish/` directory. You will find:

1. `HlsParser.CommandLine.exe` - The main executable
2. `HlsParser.bat` - A Windows batch file wrapper that lets you use the shorter `HlsParser` command
3. `HlsParser` - A shell script for Linux/macOS users that lets you use the shorter command (make it executable with `chmod +x HlsParser`)

**Using the shorter command:**

On Windows:
```
HlsParser https://example.com/master.m3u8
```

On Linux/macOS:
```
./HlsParser https://example.com/master.m3u8
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
// Note: If segment is encrypted, decryption must be handled by you
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
    public EncryptionInfo Encryption { get; set; }  // Metadata only, decryption not implemented
    public DateTimeOffset? ProgramDateTime { get; set; }
    public IList<Tag> Tags { get; set; }
}
```

#### 🔐 EncryptionInfo

Contains metadata about segment encryption.

```csharp
public class EncryptionInfo
{
    public string Method { get; set; }      // E.g., "AES-128", "SAMPLE-AES"
    public Uri KeyUri { get; set; }         // URI to fetch the decryption key
    public string Iv { get; set; }          // Initialization vector
    public string KeyFormat { get; set; }   // Format of the key
    public string KeyFormatVersions { get; set; }
}
```

## 🔄 Handling Encrypted Segments

This library parses encryption metadata but does not implement decryption. For encrypted segments, you'll need to:

1. Check if `segment.Encryption != null`
2. Download the key from `segment.Encryption.KeyUri`
3. Use the encryption method, key, and IV to decrypt the segment data
4. Implement decryption according to the HLS specification for the specific method (AES-128, SAMPLE-AES, etc.)

## 📜 License

MIT 