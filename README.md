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
- 🔌 Dependency Injection support through abstractions

## 🚀 Getting Started

### 📦 Installation

> **Note:** This package is not yet published to NuGet. We plan to publish it soon. In the meantime, you can build and reference the library directly from the source code.

Once published, you'll be able to install it using:
```
dotnet add package HlsParser
```

For Dependency Injection support:
```
dotnet add package HlsParser.Abstractions
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

### 🔌 Using Dependency Injection

```csharp
using Microsoft.Extensions.DependencyInjection;
using HlsParser.Abstractions;

// In your startup code
services.AddHlsParser();

// Or with a custom HttpClient configuration
services.AddHlsParser(client => 
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "Your User Agent");
});

// In your application code
public class MyService
{
    private readonly IHlsClient _hlsClient;

    public MyService(IHlsClient hlsClient)
    {
        _hlsClient = hlsClient;
    }

    public async Task ProcessPlaylistAsync(string url)
    {
        var playlist = await _hlsClient.GetPlaylistAsync(new Uri(url));
        
        if (playlist is IMasterPlaylist masterPlaylist)
        {
            // Process master playlist
            Console.WriteLine($"Found {masterPlaylist.Streams.Count} stream variants");
        }
        else if (playlist is IMediaPlaylist mediaPlaylist)
        {
            // Process media playlist
            Console.WriteLine($"Found {mediaPlaylist.Segments.Count} segments");
        }
    }
}
```

## 🛠️ Recent Updates

### Nullable Reference Type Support

- The library now properly supports nullable reference types in C# 8+
- The `IRendition.Uri` property has been updated to handle null values correctly when a rendition doesn't have a URI
- URI handling has been improved to gracefully handle invalid URI strings

### Bug Fixes

- Fixed an issue with URI parsing in `RenditionAdapter` that would throw exceptions when encountering invalid URI formats
- Improved error handling throughout the codebase to provide more graceful degradation

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

### 🧩 Abstractions

For dependency injection scenarios, use the interfaces in the `HlsParser.Abstractions` namespace:

```csharp
// Main interfaces
IHlsClient                // Client for downloading and parsing playlists
IHlsParser               // Parser for HLS playlists

// Model interfaces
IPlaylist                // Base interface for all playlists
IMasterPlaylist          // Interface for master playlists
IMediaPlaylist           // Interface for media playlists
IMediaSegment            // Interface for media segments
IStreamInfo              // Interface for stream variants
IRenditionGroup          // Interface for rendition groups
IRendition               // Interface for renditions
ITag                     // Interface for HLS tags
IEncryptionInfo          // Interface for encryption metadata
```

#### Adapter Pattern Implementation

The `HlsParser.Abstractions` library uses the adapter pattern to provide a clean interface layer between the core library models and client code. Key adapter implementations include:

- `PlaylistAdapter` - Base adapter for all playlist types
- `MasterPlaylistAdapter` - Adapts the core `MasterPlaylist` model to the `IMasterPlaylist` interface
- `MediaPlaylistAdapter` - Adapts the core `MediaPlaylist` model to the `IMediaPlaylist` interface
- `RenditionGroupAdapter` - Adapts the core `RenditionGroup` model to the `IRenditionGroup` interface
- `RenditionAdapter` - Adapts the core `Rendition` model to the `IRendition` interface

These adapters provide proper nullability handling and ensure that the library operates correctly in various edge cases, such as when dealing with missing or invalid URIs in renditions.

#### Nullability and Error Handling

The abstractions layer has been designed with nullable reference types in mind:

- Properties like `IRendition.Uri` are properly defined with correct nullability annotations
- Adapters implement graceful error handling to prevent exceptions when encountering invalid data
- The design allows for defensive programming practices when working with potentially missing data

Example of proper nullability handling in client code:

```csharp
foreach (var rendition in renditionGroup.Renditions)
{
    Console.WriteLine($"Rendition: {rendition.Name}");
    
    // Safe access to nullable URI
    if (rendition.Uri != null)
    {
        Console.WriteLine($"URI: {rendition.Uri}");
    }
    else
    {
        Console.WriteLine("No URI available");
    }
}
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