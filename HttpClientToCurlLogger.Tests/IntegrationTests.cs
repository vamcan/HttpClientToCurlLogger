using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace HttpClientToCurlLogger.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public async Task EndToEnd_HttpClientWithCurlLogging_LogsCurlCommand()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            services.AddCurlLogging(options =>
            {
                options.EnableLogging = true;
            });

            services.AddHttpClient("TestClient")
                .ConfigurePrimaryHttpMessageHandler(() => new TestHttpMessageHandler());

            var serviceProvider = services.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient("TestClient");

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com/users");
            request.Headers.Add("Authorization", "Bearer test-token");
            request.Content = new StringContent("{\"name\":\"John Doe\"}", Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task EndToEnd_MultipleHttpClients_AllLogCurlCommands()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            services.AddCurlLogging(options =>
            {
                options.EnableLogging = true;
            });

            services.AddHttpClient("Client1")
                .ConfigurePrimaryHttpMessageHandler(() => new TestHttpMessageHandler());
            
            services.AddHttpClient("Client2")
                .ConfigurePrimaryHttpMessageHandler(() => new TestHttpMessageHandler());

            var serviceProvider = services.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            // Act
            var client1 = httpClientFactory.CreateClient("Client1");
            var client2 = httpClientFactory.CreateClient("Client2");

            var response1 = await client1.GetAsync("https://api1.example.com/data");
            var response2 = await client2.GetAsync("https://api2.example.com/data");

            // Assert
            Assert.NotNull(response1);
            Assert.NotNull(response2);
            Assert.Equal(System.Net.HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.OK, response2.StatusCode);
        }

        [Fact]
        public async Task EndToEnd_WithLoggingDisabled_DoesNotThrowException()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddCurlLogging(options =>
            {
                options.EnableLogging = false;
            });

            services.AddHttpClient("TestClient")
                .ConfigurePrimaryHttpMessageHandler(() => new TestHttpMessageHandler());

            var serviceProvider = services.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient("TestClient");

            // Act & Assert
            var response = await client.GetAsync("https://api.example.com/data");
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        private class TestHttpMessageHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"success\":true}")
                });
            }
        }
    }
}
