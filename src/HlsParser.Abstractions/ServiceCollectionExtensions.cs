using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HlsParser.Abstractions
{
    /// <summary>
    /// Extension methods for setting up HLS Parser services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds HlsParser services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddHlsParser(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Register HttpClient if not already registered
            services.TryAddSingleton<HttpClient>();

            // Register HLS Parser services
            services.TryAddSingleton<IHlsParser, HlsParserAdapter>();
            services.TryAddSingleton<IHlsClient, HlsClientAdapter>();

            return services;
        }

        /// <summary>
        /// Adds HlsParser services to the specified <see cref="IServiceCollection" /> with a custom HttpClient.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="configureClient">A delegate to configure the HttpClient.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddHlsParser(this IServiceCollection services, Action<HttpClient> configureClient)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureClient == null)
            {
                throw new ArgumentNullException(nameof(configureClient));
            }

            // Configure and register HttpClient
            var httpClient = new HttpClient();
            configureClient(httpClient);
            services.AddSingleton(httpClient);

            // Register HLS Parser services
            services.TryAddSingleton<IHlsParser, HlsParserAdapter>();
            services.TryAddSingleton<IHlsClient, HlsClientAdapter>();

            return services;
        }
    }
} 