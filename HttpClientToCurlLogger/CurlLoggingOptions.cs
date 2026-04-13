namespace HttpClientToCurlLogger
{
    public class CurlLoggingOptions
    {
        public bool EnableLogging { get; set; }

        /// <summary>
        /// If true, formats the curl command with line breaks for better readability.
        /// Default is true.
        /// </summary>
        public bool UseMultiLineFormat { get; set; } = true;

        /// <summary>
        /// If true, adds separators and headers around the curl command.
        /// Default is true.
        /// </summary>
        public bool UseFormattedOutput { get; set; } = true;

        /// <summary>
        /// If true, writes directly to Console instead of using ILogger.
        /// This is useful when using structured logging (like Serilog with JSON formatters)
        /// that would escape the formatted output. Default is false.
        /// </summary>
        public bool UseConsoleOutput { get; set; } = false;
    }
}