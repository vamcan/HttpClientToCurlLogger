using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HttpClientToCurlLogger.Tests
{
    public class CurlLoggingHandlerTests
    {
        private readonly Mock<ILogger<CurlLoggingHandler>> _loggerMock;
        private readonly Mock<IOptions<CurlLoggingOptions>> _optionsMock;

        public CurlLoggingHandlerTests()
        {
            _loggerMock = new Mock<ILogger<CurlLoggingHandler>>();
            _optionsMock = new Mock<IOptions<CurlLoggingOptions>>();
        }

        [Fact]
        public async Task SendAsync_WhenLoggingEnabled_LogsCurlCommand()
        {
            // Arrange
            _optionsMock.Setup(o => o.Value).Returns(new CurlLoggingOptions { EnableLogging = true });
            
            var handler = new CurlLoggingHandler(_optionsMock.Object, _loggerMock.Object)
            {
                InnerHandler = new TestMessageHandler()
            };

            var client = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/users");

            // Act
            await client.SendAsync(request);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("curl -X GET")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SendAsync_WhenLoggingDisabled_DoesNotLogCurlCommand()
        {
            // Arrange
            _optionsMock.Setup(o => o.Value).Returns(new CurlLoggingOptions { EnableLogging = false });
            
            var handler = new CurlLoggingHandler(_optionsMock.Object, _loggerMock.Object)
            {
                InnerHandler = new TestMessageHandler()
            };

            var client = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/users");

            // Act
            await client.SendAsync(request);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }

        [Fact]
        public async Task SendAsync_WithPostRequestAndBody_IncludesBodyInCurl()
        {
            // Arrange
            _optionsMock.Setup(o => o.Value).Returns(new CurlLoggingOptions { EnableLogging = true });
            
            var handler = new CurlLoggingHandler(_optionsMock.Object, _loggerMock.Object)
            {
                InnerHandler = new TestMessageHandler()
            };

            var client = new HttpClient(handler);
            var content = new StringContent("{\"name\":\"John\"}", Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com/users")
            {
                Content = content
            };

            // Act
            await client.SendAsync(request);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("-d") && o.ToString().Contains("name")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SendAsync_WithHeaders_IncludesHeadersInCurl()
        {
            // Arrange
            _optionsMock.Setup(o => o.Value).Returns(new CurlLoggingOptions { EnableLogging = true });
            
            var handler = new CurlLoggingHandler(_optionsMock.Object, _loggerMock.Object)
            {
                InnerHandler = new TestMessageHandler()
            };

            var client = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/users");
            request.Headers.Add("Authorization", "Bearer token123");
            request.Headers.Add("X-Custom-Header", "CustomValue");

            // Act
            await client.SendAsync(request);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => 
                        o.ToString().Contains("-H") && 
                        o.ToString().Contains("Authorization") &&
                        o.ToString().Contains("X-Custom-Header")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SendAsync_WithDifferentHttpMethods_IncludesCorrectMethod()
        {
            // Arrange
            _optionsMock.Setup(o => o.Value).Returns(new CurlLoggingOptions { EnableLogging = true });
            
            var methods = new[] { HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete, HttpMethod.Patch };

            foreach (var method in methods)
            {
                _loggerMock.Reset();
                
                var handler = new CurlLoggingHandler(_optionsMock.Object, _loggerMock.Object)
                {
                    InnerHandler = new TestMessageHandler()
                };

                var client = new HttpClient(handler);
                var request = new HttpRequestMessage(method, "https://api.example.com/users");

                // Act
                await client.SendAsync(request);

                // Assert
                _loggerMock.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => o.ToString().Contains($"curl -X {method.Method}")),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                    Times.Once);
            }
        }

        [Fact]
        public async Task SendAsync_WithContentHeaders_IncludesContentHeadersInCurl()
        {
            // Arrange
            _optionsMock.Setup(o => o.Value).Returns(new CurlLoggingOptions { EnableLogging = true });
            
            var handler = new CurlLoggingHandler(_optionsMock.Object, _loggerMock.Object)
            {
                InnerHandler = new TestMessageHandler()
            };

            var client = new HttpClient(handler);
            var content = new StringContent("{\"test\":\"data\"}", Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com/users")
            {
                Content = content
            };

            // Act
            await client.SendAsync(request);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => 
                        o.ToString().Contains("Content-Type") &&
                        o.ToString().Contains("application/json")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SendAsync_WithEmptyBody_DoesNotIncludeDataFlag()
        {
            // Arrange
            _optionsMock.Setup(o => o.Value).Returns(new CurlLoggingOptions { EnableLogging = true });
            
            var handler = new CurlLoggingHandler(_optionsMock.Object, _loggerMock.Object)
            {
                InnerHandler = new TestMessageHandler()
            };

            var client = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com/users")
            {
                Content = new StringContent("", Encoding.UTF8)
            };

            // Act
            await client.SendAsync(request);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => !o.ToString().Contains(" -d ")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        private class TestMessageHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            }
        }
    }
}
