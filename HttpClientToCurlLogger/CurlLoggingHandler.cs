using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttpClientToCurlLogger
{
    public class CurlLoggingHandler : DelegatingHandler
    {
        private readonly ILogger<CurlLoggingHandler> _logger;
        private readonly CurlLoggingOptions _options;

        public CurlLoggingHandler(IOptions<CurlLoggingOptions> options, ILogger<CurlLoggingHandler> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (_options.EnableLogging)
            {
                var curl = await GenerateCurlCommand(request);
                LogCurlCommand(curl);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private void LogCurlCommand(string curlCommand)
        {
            if (_options.UseConsoleOutput)
            {
                // Write directly to console to avoid structured logging escaping
                if (_options.UseFormattedOutput)
                {
                    var separator = new string('=', 80);
                    Console.WriteLine();
                    Console.WriteLine(separator);
                    Console.WriteLine("cURL Command (copy-paste ready):");
                    Console.WriteLine(separator);
                    Console.WriteLine(curlCommand);
                    Console.WriteLine(separator);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(curlCommand);
                }
            }
            else
            {
                // Use logger (may be affected by structured logging formatters)
                if (_options.UseFormattedOutput)
                {
                    var separator = new string('=', 80);
                    var formattedOutput = new StringBuilder();

                    formattedOutput.AppendLine();
                    formattedOutput.AppendLine(separator);
                    formattedOutput.AppendLine("cURL Command (copy-paste ready):");
                    formattedOutput.AppendLine(separator);
                    formattedOutput.AppendLine(curlCommand);
                    formattedOutput.AppendLine(separator);

                    _logger.LogInformation(formattedOutput.ToString());
                }
                else
                {
                    _logger.LogInformation(curlCommand);
                }
            }
        }

        private async Task<string> GenerateCurlCommand(HttpRequestMessage request)
        {
            var uri = request.RequestUri;
            var method = request.Method.Method;

            var curlCommand = new StringBuilder();
            curlCommand.Append("curl");

            var lineSeparator = _options.UseMultiLineFormat ? " \\\n  " : " ";

            // Add method
            curlCommand.Append($" -X {method}");

            // Add request headers
            foreach (var header in request.Headers)
            {
                foreach (var value in header.Value)
                {
                    var escapedValue = value.Replace("\"", "\\\"");
                    curlCommand.Append($"{lineSeparator}-H \"{header.Key}: {escapedValue}\"");
                }
            }

            // Add content and content headers
            if (request.Content != null)
            {
                var contentBytes = await request.Content.ReadAsByteArrayAsync();
                var bufferedContent = new ByteArrayContent(contentBytes);

                foreach (var header in request.Content.Headers)
                {
                    bufferedContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    var headerValue = string.Join(", ", header.Value);
                    var escapedValue = headerValue.Replace("\"", "\\\"");
                    curlCommand.Append($"{lineSeparator}-H \"{header.Key}: {escapedValue}\"");
                }

                request.Content = bufferedContent;

                var contentString = Encoding.UTF8.GetString(contentBytes);
                if (!string.IsNullOrEmpty(contentString))
                {
                    var escapedContent = contentString.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
                    curlCommand.Append($"{lineSeparator}-d \"{escapedContent}\"");
                }
            }

            // Add URL
            curlCommand.Append($"{lineSeparator}\"{uri}\"");

            return curlCommand.ToString();
        }
    }
}