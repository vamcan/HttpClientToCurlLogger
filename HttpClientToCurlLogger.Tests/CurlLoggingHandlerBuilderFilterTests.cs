using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Xunit;

namespace HttpClientToCurlLogger.Tests
{
    public class CurlLoggingHandlerBuilderFilterTests
    {
        [Fact]
        public void Configure_AddsHandlerToHttpClientBuilder()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddCurlLogging(options => options.EnableLogging = true);
            services.AddHttpClient("TestClient");

            var serviceProvider = services.BuildServiceProvider();
            
            // Act
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient("TestClient");

            // Assert
            Assert.NotNull(client);
        }

        [Fact]
        public void Configure_ReturnsConfiguredBuilder()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddTransient<CurlLoggingHandler>();
            var serviceProvider = services.BuildServiceProvider();
            var filter = new CurlLoggingHandlerBuilderFilter(serviceProvider);

            var called = false;
            Action<HttpMessageHandlerBuilder> next = builder => { called = true; };

            // Act
            var result = filter.Configure(next);
            var testBuilder = new TestHttpMessageHandlerBuilder(serviceProvider);
            result(testBuilder);

            // Assert
            Assert.True(called);
        }

        private class TestHttpMessageHandlerBuilder : HttpMessageHandlerBuilder
        {
            public TestHttpMessageHandlerBuilder(IServiceProvider serviceProvider)
            {
                Services = serviceProvider;
                AdditionalHandlers = new System.Collections.Generic.List<DelegatingHandler>();
            }

            public override IServiceProvider Services { get; }
            public override string Name { get; set; }
            public override HttpMessageHandler PrimaryHandler { get; set; } = new HttpClientHandler();
            public override System.Collections.Generic.IList<DelegatingHandler> AdditionalHandlers { get; }

            public override HttpMessageHandler Build()
            {
                return PrimaryHandler;
            }
        }
    }
}
