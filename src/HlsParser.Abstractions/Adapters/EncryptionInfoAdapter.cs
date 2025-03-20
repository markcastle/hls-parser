using System;
using HlsParser.Models;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Adapter for the EncryptionInfo class that implements the IEncryptionInfo interface
    /// </summary>
    internal class EncryptionInfoAdapter : IEncryptionInfo
    {
        private readonly EncryptionInfo _encryption;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionInfoAdapter"/> class
        /// </summary>
        /// <param name="encryption">The encryption info to adapt</param>
        public EncryptionInfoAdapter(EncryptionInfo encryption)
        {
            _encryption = encryption ?? throw new ArgumentNullException(nameof(encryption));
        }

        /// <inheritdoc/>
        public string Method => _encryption.Method;

        /// <inheritdoc/>
        public Uri KeyUri => _encryption.KeyUri;

        /// <inheritdoc/>
        public string Iv => _encryption.Iv;

        /// <inheritdoc/>
        public string KeyFormat => _encryption.KeyFormat;

        /// <inheritdoc/>
        public string KeyFormatVersions => _encryption.KeyFormatVersions;
    }
} 