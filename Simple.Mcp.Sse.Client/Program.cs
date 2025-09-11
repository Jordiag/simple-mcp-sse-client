using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Simple.Mcp.Sse.Client.Extensions;
using Simple.Mcp.Sse.Client.HostedServices;


namespace Simple.Mcp.Sse.Client;

static class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            IHost host = CreateHostBuilder(args).Build();

            Console.WriteLine("🎯 Starting Semantic Kernel MCP Client...");

            await host.RunAsync();
        }
        catch(Exception ex)
        {
            Console.WriteLine($"❌ Application startup failed: {ex.Message}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            })
            .ConfigureServices((context, services) =>
            {
                // Add application services
                services.AddApplicationServices(context.Configuration);

                // Add the main application hosted service
                services.AddHostedService<ApplicationHostedService>();
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);

                // Reduce noise from some verbose libraries
                logging.AddFilter("Microsoft.SemanticKernel", LogLevel.Warning);
                logging.AddFilter("System.Net.Http", LogLevel.Warning);
            })
            .UseConsoleLifetime();
}
