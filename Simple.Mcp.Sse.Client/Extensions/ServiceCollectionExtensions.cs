using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
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

            // Configure AI service based on available configuration
            if (!string.IsNullOrEmpty(kernelConfig.OpenAI.ApiKey))
            {
                logger.LogInformation("Configuring OpenAI service");
                kernelBuilder.AddOpenAIChatCompletion(
                    kernelConfig.OpenAI.ModelId,
                    kernelConfig.OpenAI.ApiKey);
            }
            else if (!string.IsNullOrEmpty(kernelConfig.AzureOpenAI.ApiKey) && !string.IsNullOrEmpty(kernelConfig.AzureOpenAI.Endpoint))
            {
                logger.LogInformation("Configuring Azure OpenAI service");
                kernelBuilder.AddAzureOpenAIChatCompletion(
                    kernelConfig.AzureOpenAI.DeploymentName,
                    kernelConfig.AzureOpenAI.Endpoint,
                    kernelConfig.AzureOpenAI.ApiKey);
            }
            else
            {
                throw new InvalidOperationException(
                    "No AI service configuration found. Please configure either OpenAI or Azure OpenAI in appsettings.json");
            }

            Kernel kernel = kernelBuilder.Build();

            // Register MCP Tools Plugin
            IMcpService mcpService = serviceProvider.GetRequiredService<IMcpService>();
            ILogger<McpToolsPlugin> mcpLogger = serviceProvider.GetRequiredService<ILogger<McpToolsPlugin>>();
            McpToolsPlugin mcpPlugin = new(mcpService, mcpLogger);

            // Initialize plugin and add to kernel
            Task.Run(async () =>
            {
                try
                {
                    await mcpPlugin.InitializeAsync();
                    logger.LogInformation("MCP Tools Plugin initialized successfully");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to initialize MCP Tools Plugin");
                }
            });

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