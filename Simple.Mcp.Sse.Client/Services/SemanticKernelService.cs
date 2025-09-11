using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Simple.Mcp.Sse.Client.Services;

public interface ISemanticKernelService
{
    Task<string> ProcessPromptAsync(string prompt, CancellationToken cancellationToken = default);
    Task<string> ProcessPromptWithHistoryAsync(string prompt, ChatHistory? history = null, CancellationToken cancellationToken = default);
    ChatHistory CreateChatHistory();
}

public class SemanticKernelService : ISemanticKernelService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly ILogger<SemanticKernelService> _logger;

    public SemanticKernelService(
        Kernel kernel,
        IChatCompletionService chatCompletionService,
        ILogger<SemanticKernelService> logger)
    {
        _kernel = kernel;
        _chatCompletionService = chatCompletionService;
        _logger = logger;
    }

    public async Task<string> ProcessPromptAsync(string prompt, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing prompt: {Prompt}", prompt.Length > 100 ? prompt[..100] + "..." : prompt);

            OpenAIPromptExecutionSettings settings = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                Temperature = 0.7,
                MaxTokens = 2000
            };

            KernelArguments arguments = new(settings);
            FunctionResult result = await _kernel.InvokePromptAsync(prompt, arguments, cancellationToken: cancellationToken);
            string response = result.ToString();

            _logger.LogInformation("Generated response of length: {Length}", response.Length);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing prompt");
            throw;
        }
    }

    public async Task<string> ProcessPromptWithHistoryAsync(string prompt, ChatHistory? history = null, CancellationToken cancellationToken = default)
    {
        try
        {
            history ??= CreateChatHistory();
            history.AddUserMessage(prompt);

            _logger.LogInformation("Processing prompt with history. Messages in history: {Count}", history.Count);

            OpenAIPromptExecutionSettings settings = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                Temperature = 0.7,
                MaxTokens = 2000
            };

            ChatMessageContent result = await _chatCompletionService.GetChatMessageContentAsync(history, settings, _kernel, cancellationToken);
            
            history.AddAssistantMessage(result.Content ?? string.Empty);
            
            _logger.LogInformation("Generated response of length: {Length}", result.Content?.Length ?? 0);
            return result.Content ?? "No response generated";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing prompt with history");
            throw;
        }
    }

    public ChatHistory CreateChatHistory()
    {
        ChatHistory history = new();
        history.AddSystemMessage(
            "You are a helpful AI assistant with access to various tools through the Model Context Protocol (MCP). " +
            "When users ask for specific tasks, you can use the available MCP tools to help them. " +
            "Always start by listing available tools if you're not sure what tools are available, " +
            "and provide clear explanations of what you're doing when using tools.");
        
        return history;
    }
}