using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Xunit;

namespace HttpClientToCurlLogger.Tests
{
    public class CurlLoggingExtensionsTests
    {
        [Fact]
        public void AddCurlLogging_RegistersRequiredServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();

            // Act
            services.AddCurlLogging(options =>
            {
                options.EnableLogging = true;
            });

            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var handler = serviceProvider.GetService<CurlLoggingHandler>();
            Assert.NotNull(handler);

            var filter = serviceProvider.GetService<IHttpMessageHandlerBuilderFilter>();
            Assert.NotNull(filter);
            Assert.IsType<CurlLoggingHandlerBuilderFilter>(filter);
        }

        [Fact]
        public void AddCurlLogging_ConfiguresOptions()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();

            // Act
            services.AddCurlLogging(options =>
            {
                options.EnableLogging = true;
            });

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<Microsoft.Extensions.Options.IOptions<CurlLoggingOptions>>();

            // Assert
            Assert.NotNull(options);
            Assert.True(options.Value.EnableLogging);
        }

        [Fact]
        public void AddCurlLogging_WithDisabledLogging_ConfiguresCorrectly()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();

            // Act
            services.AddCurlLogging(options =>
            {
                options.EnableLogging = false;
            });

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<Microsoft.Extensions.Options.IOptions<CurlLoggingOptions>>();

            // Assert
            Assert.NotNull(options);
            Assert.False(options.Value.EnableLogging);
        }

        [Fact]
        public void AddCurlLogging_ReturnsServiceCollection()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = services.AddCurlLogging(options =>
            {
                options.EnableLogging = true;
            });

            // Assert
            Assert.Same(services, result);
        }
    }
}
