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
                _logger.LogInformation(curl);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GenerateCurlCommand(HttpRequestMessage request)
        {
            var uri = request.RequestUri;
            var method = request.Method.Method;

            var curlCommand = new StringBuilder($"curl -X {method}");

            foreach (var header in request.Headers)
            {
                foreach (var value in header.Value)
                {
                    curlCommand.Append($" -H \"{header.Key}: {value}\"");
                }
            }

            if (request.Content != null)
            {
                var contentBytes = await request.Content.ReadAsByteArrayAsync();
                var bufferedContent = new ByteArrayContent(contentBytes);

                foreach (var header in request.Content.Headers)
                {
                    bufferedContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    curlCommand.Append($" -H \"{header.Key}: {string.Join(", ", header.Value)}\"");
                }

                request.Content = bufferedContent;

                var contentString = Encoding.UTF8.GetString(contentBytes);
                if (!string.IsNullOrEmpty(contentString))
                {
                    contentString = contentString.Replace("\"", "\\\"");
                    curlCommand.Append($" -d \"{contentString}\"");
                }
            }

            curlCommand.Append($" \"{uri}\"");

            return curlCommand.ToString();
        }
    }
}