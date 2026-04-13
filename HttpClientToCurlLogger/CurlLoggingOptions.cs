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
    }
}