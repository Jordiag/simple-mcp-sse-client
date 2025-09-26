using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Simple.Mcp.Http.Client.Plugins;

public class McpToolsPlugin
{
    private readonly Services.IMcpService _mcpService;
    private readonly ILogger<McpToolsPlugin> _logger;
    private readonly Dictionary<string, ModelContextProtocol.Client.McpClientTool> _availableTools = new();

    public McpToolsPlugin(Services.IMcpService mcpService, ILogger<McpToolsPlugin> logger)
    {
        _mcpService = mcpService;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            IList<ModelContextProtocol.Client.McpClientTool> tools = await _mcpService.GetToolsAsync();
            _availableTools.Clear();

            foreach(ModelContextProtocol.Client.McpClientTool tool in tools)
            {
                _availableTools[tool.Name] = tool;
                _logger.LogDebug("Registered MCP tool: {ToolName} - {Description}", tool.Name, tool.Description);
            }

            _logger.LogInformation("Initialized MCP Tools Plugin with {ToolCount} tools", _availableTools.Count);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MCP Tools Plugin");
            throw;
        }
    }

    [KernelFunction]
    [Description("Lists all available MCP tools with their descriptions")]
    public async Task<string> ListAvailableTools()
    {
        if(!_availableTools.Any())
        {
            await InitializeAsync();
        }

        List<string> toolsList = _availableTools.Values
            .Select(tool => $"- {tool.Name}: {tool.Description}")
            .ToList();

        return $"Available MCP Tools ({toolsList.Count}):\n" + string.Join("\n", toolsList);
    }

    [KernelFunction]
    [Description("Invokes a specific MCP tool with the provided arguments")]
    public async Task<string> InvokeMcpTool(
        [Description("The name of the MCP tool to invoke")] string toolName,
        [Description("JSON string containing the arguments for the tool")] string argumentsJson = "{}")
    {
        try
        {
            if(!_availableTools.ContainsKey(toolName))
            {
                return $"Error: Tool '{toolName}' not found. Use ListAvailableTools to see available tools.";
            }

            Dictionary<string, object> arguments;
            try
            {
                arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(argumentsJson)
                           ?? new Dictionary<string, object>();
            }
            catch(JsonException ex)
            {
                return $"Error: Invalid JSON in arguments: {ex.Message}";
            }

            string result = await _mcpService.InvokeToolAsync(toolName, arguments);
            return result;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error invoking MCP tool {ToolName}", toolName);
            return $"Error invoking tool '{toolName}': {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Gets detailed information about a specific MCP tool including its input schema")]
    public async Task<string> GetToolInfo(
        [Description("The name of the MCP tool to get information about")] string toolName)
    {
        if(!_availableTools.Any())
        {
            await InitializeAsync();
        }

        if(!_availableTools.TryGetValue(toolName, out ModelContextProtocol.Client.McpClientTool? tool))
        {
            return $"Tool '{toolName}' not found. Use ListAvailableTools to see available tools.";
        }

        string info = $"Tool: {tool.Name}\n";
        info += $"Description: {tool.Description}\n";
        info += "Input Schema: Schema information not available from current MCP client version";

        return info;
    }
}