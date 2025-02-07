using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace HttpClientToCurlLogger
{
    public static class CurlLoggingExtensions
    {
        public static IServiceCollection AddCurlLogging(this IServiceCollection services,
            Action<CurlLoggingOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddTransient<CurlLoggingHandler>();
            services.AddSingleton<IHttpMessageHandlerBuilderFilter, CurlLoggingHandlerBuilderFilter>();
            return services;
        }
    }
}