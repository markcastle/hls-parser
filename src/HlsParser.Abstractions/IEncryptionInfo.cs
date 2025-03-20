using System;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Interface for encryption information for a media segment
    /// </summary>
    public interface IEncryptionInfo
    {
        /// <summary>
        /// Gets the encryption method (AES-128, SAMPLE-AES, etc.)
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the URI of the key file
        /// </summary>
        Uri KeyUri { get; }

        /// <summary>
        /// Gets the initialization vector if provided
        /// </summary>
        string Iv { get; }

        /// <summary>
        /// Gets the key format if provided
        /// </summary>
        string KeyFormat { get; }

        /// <summary>
        /// Gets the key format versions if provided
        /// </summary>
        string KeyFormatVersions { get; }
    }
} 