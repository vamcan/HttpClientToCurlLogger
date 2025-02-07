# HttpClientToCurlLogger

A .NET Core library to log `HttpClient` requests as cURL commands. Automatically logs to the console when enabled.

## Features

- **Enable/Disable Logging**: Toggle cURL logging via configuration.
- **Automatic Registration**: Works out-of-the-box with single or multiple `HttpClient` instances.
- **Development Focus**: Ideal for debugging API requests during development.



## Usage

```bash
public void ConfigureServices(IServiceCollection services)
{
    services.AddCurlLogging(options =>
    {
        options.EnableLogging = true; // Set to false to disable
    });

    // Register your HttpClient(s)
    services.AddHttpClient("MyClient");
}
