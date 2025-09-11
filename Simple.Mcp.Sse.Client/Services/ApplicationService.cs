using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Simple.Mcp.Sse.Client.Services;

public interface IApplicationService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}

public class ApplicationService : IApplicationService
{
    private readonly ISemanticKernelService _semanticKernelService;
    private readonly IMcpService _mcpService;
    private readonly ILogger<ApplicationService> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public ApplicationService(
        ISemanticKernelService semanticKernelService,
        IMcpService mcpService,
        ILogger<ApplicationService> logger,
        IHostApplicationLifetime applicationLifetime)
    {
        _semanticKernelService = semanticKernelService;
        _mcpService = mcpService;
        _logger = logger;
        _applicationLifetime = applicationLifetime;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Console.WriteLine("🚀 Semantic Kernel MCP Client");
            Console.WriteLine("==========================================");

            // Test MCP connection
            Console.WriteLine("\n🔗 Testing MCP server connection...");
            IList<ModelContextProtocol.Client.McpClientTool> tools = await _mcpService.GetToolsAsync();
            Console.WriteLine($"✅ Connected successfully! Found {tools.Count} available tools:");

            foreach(ModelContextProtocol.Client.McpClientTool tool in tools)
            {
                Console.WriteLine($"   • {tool.Name}: {tool.Description}");
            }

            Console.WriteLine("\n💡 You can now interact with the AI assistant. Available commands:");
            Console.WriteLine("   • Type your questions or requests normally");
            Console.WriteLine("   • Type 'list tools' to see available MCP tools");
            Console.WriteLine("   • Type 'exit' or 'quit' to stop");
            Console.WriteLine("\n" + new string('=', 50));

            ChatHistory chatHistory = _semanticKernelService.CreateChatHistory();

            while(!cancellationToken.IsCancellationRequested)
            {
                Console.Write("\n👤 You: ");
                string? input = Console.ReadLine();

                if(string.IsNullOrWhiteSpace(input))
                    continue;

                if(input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                try
                {
                    // Create a cancellation token source for the loading indicator
                    using CancellationTokenSource loadingCancellation = new();
                    
                    // Show a loading indicator
                    Task loadingTask = ShowLoadingIndicator(loadingCancellation.Token);

                    string response = await _semanticKernelService.ProcessPromptWithHistoryAsync(input, chatHistory, cancellationToken);

                    // Cancel and wait for loading indicator to stop
                    loadingCancellation.Cancel();
                    try
                    {
                        await loadingTask;
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected when we cancel the loading
                    }
                    
                    // Clear the loading line and show the response
                    Console.Write("\r" + new string(' ', 50) + "\r"); // Clear the line
                    Console.WriteLine($"🤖 Assistant: {response}");
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"\r❌ Error: {ex.Message}");
                    _logger.LogError(ex, "Error processing user input");
                }
            }

            Console.WriteLine("\n👋 Thank you for using the Semantic Kernel MCP Client!");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Critical error in application service");
            Console.WriteLine($"❌ Critical Error: {ex.Message}");
        }
        finally
        {
            _applicationLifetime.StopApplication();
        }
    }

    private static async Task ShowLoadingIndicator(CancellationToken cancellationToken)
    {
        char[] loadingChars = ['⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏'];
        int index = 0;

        try
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                Console.Write($"\r🤖 Assistant: {loadingChars[index % loadingChars.Length]} Thinking...");
                index++;
                await Task.Delay(100, cancellationToken);
            }
        }
        catch(OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
    }
}