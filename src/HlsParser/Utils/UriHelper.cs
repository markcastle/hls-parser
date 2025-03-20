using System;

namespace HlsParser.Utils
{
    /// <summary>
    /// Helper class for working with URIs
    /// </summary>
    public static class UriHelper
    {
        /// <summary>
        /// Resolves a relative URI against a base URI
        /// </summary>
        /// <param name="baseUri">The base URI</param>
        /// <param name="relativeUri">The relative URI</param>
        /// <returns>The resolved URI</returns>
        public static Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if (baseUri == null)
                throw new ArgumentNullException(nameof(baseUri));

            if (string.IsNullOrEmpty(relativeUri))
                return baseUri;

            // Check if the relative URI is already absolute
            if (Uri.TryCreate(relativeUri, UriKind.Absolute, out var absoluteUri))
                return absoluteUri;

            // Resolve the relative URI against the base URI
            return new Uri(baseUri, relativeUri);
        }

        /// <summary>
        /// Gets the base URI (directory) of the specified URI
        /// </summary>
        /// <param name="uri">The URI</param>
        /// <returns>The base URI</returns>
        public static Uri GetBaseUri(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            string baseUriString = uri.AbsoluteUri;
            int lastSlash = baseUriString.LastIndexOf('/');
            if (lastSlash > 0)
            {
                baseUriString = baseUriString.Substring(0, lastSlash + 1);
                return new Uri(baseUriString);
            }

            return uri;
        }

        /// <summary>
        /// Gets the file name from the specified URI
        /// </summary>
        /// <param name="uri">The URI</param>
        /// <returns>The file name</returns>
        public static string GetFileName(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            string path = uri.AbsolutePath;
            
            // If path is only a slash, return empty string
            if (path == "/")
                return string.Empty;
                
            int lastSlash = path.LastIndexOf('/');
            if (lastSlash >= 0 && lastSlash < path.Length - 1)
            {
                return path.Substring(lastSlash + 1);
            }

            return path;
        }
    }
} 