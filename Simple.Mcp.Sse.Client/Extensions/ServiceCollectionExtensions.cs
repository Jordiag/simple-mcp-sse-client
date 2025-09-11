using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Simple.Mcp.Sse.Client.Configuration;
using Simple.Mcp.Sse.Client.Plugins;
using Simple.Mcp.Sse.Client.Services;

namespace Simple.Mcp.Sse.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.Configure<SemanticKernelConfiguration>(configuration.GetSection("SemanticKernel"));
        services.Configure<McpServerConfiguration>(configuration.GetSection("McpServer"));

        // MCP Service
        services.AddSingleton<IMcpService, McpService>();

        // Semantic Kernel setup
        services.AddTransient(serviceProvider =>
        {
            SemanticKernelConfiguration kernelConfig = serviceProvider.GetRequiredService<IOptions<SemanticKernelConfiguration>>().Value;
            ILogger<Kernel> logger = serviceProvider.GetRequiredService<ILogger<Kernel>>();

            IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

            // Get API key from environment variable
            string? openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            string? azureOpenAiApiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");

            // Configure AI service based on available configuration
            if (!string.IsNullOrEmpty(openAiApiKey))
            {
                logger.LogInformation("Configuring OpenAI service with environment variable API key");
                kernelBuilder.AddOpenAIChatCompletion(
                    kernelConfig.OpenAI.ModelId,
                    openAiApiKey);
            }
            else if (!string.IsNullOrEmpty(azureOpenAiApiKey) && !string.IsNullOrEmpty(kernelConfig.AzureOpenAI.Endpoint))
            {
                logger.LogInformation("Configuring Azure OpenAI service with environment variable API key");
                kernelBuilder.AddAzureOpenAIChatCompletion(
                    kernelConfig.AzureOpenAI.DeploymentName,
                    kernelConfig.AzureOpenAI.Endpoint,
                    azureOpenAiApiKey);
            }
            else
            {
                throw new InvalidOperationException(
                    "No AI service configuration found. Please set the OPENAI_API_KEY or AZURE_OPENAI_API_KEY environment variable.");
            }

            Kernel kernel = kernelBuilder.Build();

            // Register MCP Tools Plugin with proper initialization
            IMcpService mcpService = serviceProvider.GetRequiredService<IMcpService>();
            ILogger<McpToolsPlugin> mcpLogger = serviceProvider.GetRequiredService<ILogger<McpToolsPlugin>>();
            McpToolsPlugin mcpPlugin = new(mcpService, mcpLogger);

            // Initialize plugin synchronously
            try
            {
                mcpPlugin.InitializeAsync().Wait();
                logger.LogInformation("MCP Tools Plugin initialized successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to initialize MCP Tools Plugin");
                throw;
            }

            kernel.Plugins.AddFromObject(mcpPlugin, "McpTools");

            logger.LogInformation("Semantic Kernel configured successfully");
            return kernel;
        });

        // Chat Completion Service
        services.AddTransient(serviceProvider =>
        {
            Kernel kernel = serviceProvider.GetRequiredService<Kernel>();
            return kernel.GetRequiredService<IChatCompletionService>();
        });

        // Application services
        services.AddScoped<ISemanticKernelService, SemanticKernelService>();
        services.AddScoped<IApplicationService, ApplicationService>();

        return services;
    }
}