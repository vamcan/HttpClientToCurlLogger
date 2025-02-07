using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace HttpClientToCurlLogger
{
    public class CurlLoggingHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public CurlLoggingHandlerBuilderFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            return builder =>
            {
                next(builder);
                var handler = _serviceProvider.GetRequiredService<CurlLoggingHandler>();
                builder.AdditionalHandlers.Add(handler);
            };
        }
    }
}