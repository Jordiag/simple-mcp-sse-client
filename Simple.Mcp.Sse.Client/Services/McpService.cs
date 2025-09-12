using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;

namespace Simple.Mcp.Sse.Client.Services;

public interface IMcpService
{
    Task<IMcpClient> GetClientAsync();
    Task<IList<McpClientTool>> GetToolsAsync();
    Task<string> InvokeToolAsync(string toolName, Dictionary<string, object> arguments);
    ValueTask DisposeAsync();
}

public class McpService : IMcpService, IAsyncDisposable
{
    private readonly Configuration.McpServerConfiguration _configuration;
    private readonly ILogger<McpService> _logger;
    private IMcpClient? _client;
    private readonly SemaphoreSlim _initializationSemaphore = new(1, 1);

    public McpService(
        IOptions<Configuration.McpServerConfiguration> configuration,
        ILogger<McpService> logger)
    {
        _configuration = configuration.Value;
        _logger = logger;
    }

    public async Task<IMcpClient> GetClientAsync()
    {
        if(_client != null)
            return _client;

        await _initializationSemaphore.WaitAsync();
        try
        {
            if(_client != null)
                return _client;

            _logger.LogInformation("Connecting to MCP server at {Endpoint}", _configuration.Endpoint);

            SseClientTransportOptions options = new()
            {
                Endpoint = new Uri(_configuration.Endpoint),
                Name = _configuration.Name,
                ConnectionTimeout = TimeSpan.FromSeconds(_configuration.ConnectionTimeoutSeconds)
            };

            SseClientTransport transport = new(options);
            _client = await McpClientFactory.CreateAsync(transport);

            _logger.LogInformation("Successfully connected to MCP server");
            return _client;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to MCP server");
            throw;
        }
        finally
        {
            _initializationSemaphore.Release();
        }
    }

    public async Task<IList<McpClientTool>> GetToolsAsync()
    {
        try
        {
            IMcpClient client = await GetClientAsync();
            IList<McpClientTool> tools = await client.ListToolsAsync();
            _logger.LogInformation("Retrieved {ToolCount} tools from MCP server", tools.Count);
            return tools;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve tools from MCP server");
            throw;
        }
    }

    public async Task<string> InvokeToolAsync(string toolName, Dictionary<string, object> arguments)
    {
        try
        {
            IMcpClient client = await GetClientAsync();

            _logger.LogInformation("Invoking MCP tool {ToolName} with arguments: {Arguments}",
                toolName, System.Text.Json.JsonSerializer.Serialize(arguments));

            // Use dynamic type to handle the return without knowing exact type
            ModelContextProtocol.Protocol.CallToolResult? result = await client.CallToolAsync(toolName, arguments);

            // Check if there's an error (handle different possible error representations)
            if(result != null && (result.IsError == true || (result.GetType().GetProperty("Error")?.GetValue(result) != null)))
            {
                string errorMessage = "MCP tool returned an error";
                _logger.LogError(errorMessage);
                return errorMessage;
            }

            string resultText;
            if(result == null || result.Content == null)
            {
                string errorMessage = "MCP tool returned no content";
                _logger.LogError(errorMessage);
                return errorMessage;
            }
            else if (result.Content is IEnumerable<string> strContent)
            {
                resultText = string.Join(", ", strContent);
            }
            else
            {
                resultText = System.Text.Json.JsonSerializer.Serialize(result.Content);
            }

            _logger.LogInformation("MCP tool {ToolName} executed successfully", toolName);
            return resultText;
        }
        catch(Exception ex)
        {
            string errorMessage = $"Failed to invoke MCP tool '{toolName}': {ex.Message}";
            _logger.LogError(ex, errorMessage);
            return errorMessage;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if(_client != null)
        {
            await _client.DisposeAsync();
            _client = null;
        }
        _initializationSemaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}