using Xunit;

namespace HttpClientToCurlLogger.Tests
{
    public class CurlLoggingOptionsTests
    {
        [Fact]
        public void EnableLogging_DefaultsToFalse()
        {
            // Arrange & Act
            var options = new CurlLoggingOptions();

            // Assert
            Assert.False(options.EnableLogging);
        }

        [Fact]
        public void EnableLogging_CanBeSetToTrue()
        {
            // Arrange
            var options = new CurlLoggingOptions();

            // Act
            options.EnableLogging = true;

            // Assert
            Assert.True(options.EnableLogging);
        }

        [Fact]
        public void EnableLogging_CanBeSetToFalse()
        {
            // Arrange
            var options = new CurlLoggingOptions
            {
                EnableLogging = true
            };

            // Act
            options.EnableLogging = false;

            // Assert
            Assert.False(options.EnableLogging);
        }
    }
}
