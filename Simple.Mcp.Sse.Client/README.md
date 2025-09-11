# Professional Semantic Kernel MCP Client

A sophisticated AI assistant that bridges Model Context Protocol (MCP) tools with Microsoft Semantic Kernel, enabling dynamic tool discovery and intelligent agent-based interactions.

## ?? Features

- **Professional Architecture**: Built with dependency injection, hosted services, and structured logging
- **Dynamic Tool Loading**: Automatically discovers and loads all available MCP tools
- **Semantic Kernel Integration**: Uses Microsoft Semantic Kernel for intelligent conversation handling
- **Flexible AI Provider Support**: Works with both OpenAI and Azure OpenAI
- **Interactive Chat Interface**: Professional console-based chat experience
- **Robust Error Handling**: Comprehensive error handling and logging throughout
- **Configuration Management**: JSON-based configuration with environment variable support

## ?? Prerequisites

- .NET 8.0 SDK or later
- An OpenAI API key OR Azure OpenAI service endpoint
- Access to an MCP server

## ?? Configuration

1. **Configure AI Service**: Edit `appsettings.json` to add your AI service credentials:

   **For OpenAI:**
   ```json
   {
     "SemanticKernel": {
       "OpenAI": {
         "ApiKey": "your-openai-api-key-here",
         "ModelId": "gpt-4"
       }
     }
   }
   ```

   **For Azure OpenAI:**
   ```json
   {
     "SemanticKernel": {
       "AzureOpenAI": {
         "ApiKey": "your-azure-openai-key",
         "Endpoint": "https://your-resource.openai.azure.com/",
         "DeploymentName": "gpt-4"
       }
     }
   }
   ```

2. **Configure MCP Server**: Update the MCP server settings in `appsettings.json`:
   ```json
   {
     "McpServer": {
       "Endpoint": "http://your-mcp-server-url/mcp",
       "Name": "LocalHttpClient",
       "ConnectionTimeoutSeconds": 15
     }
   }
   ```

## ????? Running the Application

1. **Build the project:**
   ```bash
   dotnet build
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Alternative: Using environment variables:**
   ```bash
   export SemanticKernel__OpenAI__ApiKey="your-api-key"
   dotnet run
   ```

## ?? Usage

Once running, you can interact with the AI assistant in natural language:

- **General conversation**: Ask questions or request help
- **Tool discovery**: Ask "what tools are available?" or "list tools"
- **Tool usage**: The AI will automatically use appropriate MCP tools based on your requests
- **Exit**: Type "exit" or "quit" to stop the application

### Example Interactions

```
?? You: What tools do you have available?
?? Assistant: I'll check what MCP tools are available for you...

?? You: Can you help me analyze this data using your tools?
?? Assistant: I'll use the appropriate analysis tools to help you...

?? You: Please create a report based on the current data
?? Assistant: I'll generate a report using the available MCP tools...
```

## ??? Architecture

The application follows a clean, professional architecture:

```
Simple.Mcp.Sse.Client/
??? Configuration/          # Configuration models
??? Extensions/            # Dependency injection extensions
??? HostedServices/        # Background services
??? Plugins/              # Semantic Kernel plugins
??? Services/             # Application services
?   ??? IMcpService       # MCP client abstraction
?   ??? ISemanticKernelService # SK abstraction
?   ??? IApplicationService    # Main app orchestration
??? Program.cs            # Application entry point
??? appsettings.json     # Configuration file
```

### Key Components

1. **McpService**: Manages connection to MCP server and tool invocation
2. **McpToolsPlugin**: Bridges MCP tools as Semantic Kernel functions
3. **SemanticKernelService**: Handles AI conversations with automatic tool calling
4. **ApplicationService**: Orchestrates the user experience

## ?? Customization

### Adding Custom Plugins

You can extend the application by adding custom Semantic Kernel plugins:

```csharp
// In ServiceCollectionExtensions.cs
kernel.Plugins.AddFromType<YourCustomPlugin>("CustomPlugin");
```

### Modifying Tool Behavior

Customize how MCP tools are presented to the kernel by modifying `McpToolsPlugin.cs`.

### Configuration Options

All configuration is centralized in `appsettings.json` and can be overridden with environment variables using the pattern: `Section__Property`.

## ?? Logging

The application provides structured logging at different levels:

- **Information**: General application flow and successful operations
- **Warning**: Non-critical issues and fallback behaviors
- **Error**: Errors that don't crash the application
- **Debug**: Detailed debugging information (disabled by default)

## ?? Error Handling

The application includes comprehensive error handling:

- Connection failures to MCP server are logged and reported
- Tool invocation errors are gracefully handled
- AI service failures include helpful error messages
- Configuration issues are detected at startup

## ?? Dependencies

- **Microsoft.SemanticKernel**: Core AI orchestration
- **ModelContextProtocol**: MCP client implementation
- **Microsoft.Extensions.Hosting**: Professional application hosting
- **Microsoft.Extensions.Logging**: Structured logging
- **Microsoft.Extensions.Configuration**: Configuration management

## ?? Security Considerations

- Store API keys securely using environment variables or Azure Key Vault
- Validate MCP server certificates in production
- Monitor and log tool usage for security auditing
- Implement rate limiting for production deployments

## ?? Next Steps

This professional foundation can be extended with:

- Web API interface for remote access
- Database integration for conversation history
- Multiple MCP server support
- Custom tool authentication
- Metrics and monitoring integration
- Containerization for deployment