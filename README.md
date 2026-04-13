# HttpClientToCurlLogger

[![NuGet](https://img.shields.io/nuget/v/HttpClientToCurlLogger.svg)](https://www.nuget.org/packages/HttpClientToCurlLogger/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A lightweight .NET library that automatically logs all `HttpClient` requests as cURL commands. Perfect for debugging, testing, and sharing API requests during development.

## Features

**Automatic Logging** - Captures all `HttpClient` requests automatically  
**cURL Format** - Generates ready-to-use cURL commands  
**Easy Integration** - Single line of configuration  
**Toggle On/Off** - Enable or disable logging via configuration  
**Multiple Clients** - Works with all registered `HttpClient` instances  
**Headers & Body** - Includes headers and request body in the cURL output  
**.NET Standard 2.0** - Compatible with .NET Core 2.0+ and .NET Framework 4.6.1+

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package HttpClientToCurlLogger
```

Or via Package Manager Console:

```powershell
Install-Package HttpClientToCurlLogger
```

## Usage

### Basic Setup

Add the following to your `Startup.cs` or `Program.cs`:

```csharp
using HttpClientToCurlLogger;

public void ConfigureServices(IServiceCollection services)
{
    // Add cURL logging
    services.AddCurlLogging(options =>
    {
        options.EnableLogging = true; // Set to false to disable
    });

    // Register your HttpClient(s)
    services.AddHttpClient("MyClient");
    services.AddHttpClient<IMyService, MyService>();
}
```

### ASP.NET Core Minimal API (.NET 6+)

```csharp
using HttpClientToCurlLogger;

var builder = WebApplication.CreateBuilder(args);

// Add cURL logging
builder.Services.AddCurlLogging(options =>
{
    options.EnableLogging = true;
});

// Register HttpClient
builder.Services.AddHttpClient("MyClient");

var app = builder.Build();
app.Run();
```

### Example Output

When you make an HTTP request:

```csharp
var client = httpClientFactory.CreateClient("MyClient");
await client.PostAsJsonAsync("https://api.example.com/users", new { name = "John" });
```

You'll see in your logs:

```bash
curl -X POST -H "Content-Type: application/json; charset=utf-8" -d "{\"name\":\"John\"}" "https://api.example.com/users"
```

You can copy this cURL command directly to your terminal or Postman!

The output will look like this:

```
================================================================================
cURL Command (copy-paste ready):
================================================================================
curl -X POST \
  -H "ApiKey: your-api-key" \
  -H "Content-Type: application/json; charset=utf-8" \
  -d "{\"name\":\"John\"}" \
  "https://api.example.com/users"
================================================================================
```

## Configuration Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `EnableLogging` | `bool` | `false` | Enable or disable cURL logging |
| `UseMultiLineFormat` | `bool` | `true` | Format curl command with line breaks for better readability |
| `UseFormattedOutput` | `bool` | `true` | Add separators and headers around the curl command |

### Advanced Configuration

```csharp
services.AddCurlLogging(options =>
{
    options.EnableLogging = true;
    options.UseMultiLineFormat = true;  // Multi-line format with \ continuation
    options.UseFormattedOutput = true;  // With separators and headers
});
```

For compact single-line output (useful for structured logging):

```csharp
services.AddCurlLogging(options =>
{
    options.EnableLogging = true;
    options.UseMultiLineFormat = false;  // Single line
    options.UseFormattedOutput = false;  // No separators
});
```

### Environment-Based Configuration

You can control logging based on environment:

```csharp
services.AddCurlLogging(options =>
{
    options.EnableLogging = builder.Environment.IsDevelopment();
});
```

Or use configuration files (`appsettings.json`):

```json
{
  "CurlLogging": {
    "EnableLogging": true,`n    "UseMultiLineFormat": true,`n    "UseFormattedOutput": true
  }
}
```

```csharp
services.AddCurlLogging(options =>
{
    configuration.GetSection("CurlLogging").Bind(options);
});
```

## How It Works

The library uses an `IHttpMessageHandlerBuilderFilter` to automatically inject a `DelegatingHandler` into all registered `HttpClient` instances. This handler intercepts requests and logs them as cURL commands using the configured logger (typically `ILogger<T>`).

## Requirements

- .NET Standard 2.0 or higher
- Microsoft.Extensions.Http 2.1.0+
- Microsoft.Extensions.Logging.Abstractions 2.1.0+

## Supported Frameworks

- .NET Core 2.0+
- .NET 5, 6, 7, 8+
- .NET Framework 4.6.1+

## Use Cases

- **Debugging** - Quickly see what requests your application is making
- **Testing** - Copy cURL commands to test APIs manually
- **Documentation** - Share API request examples with your team
- **Troubleshooting** - Diagnose integration issues with external APIs

## Best Practices

1. **Disable in Production** - Only enable logging in development/staging environments
2. **Sensitive Data** - Be aware that headers and body content are logged (including auth tokens)
3. **Performance** - Minimal overhead, but consider disabling for high-throughput scenarios

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Author

[Reza Ghasemi](https://github.com/vamcan)

## Repository

[https://github.com/vamcan/HttpClientToCurlLogger](https://github.com/vamcan/HttpClientToCurlLogger)
