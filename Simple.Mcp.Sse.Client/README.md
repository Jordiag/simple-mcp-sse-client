# Semantic Kernel MCP Client

A sophisticated AI assistant that bridges Model Context Protocol (MCP) tools with Microsoft Semantic Kernel, enabling dynamic tool discovery and intelligent agent-based interactions.

## 🚀 Features

- **Simple but good Architecture**: Built with dependency injection, hosted services, and structured logging
- **Dynamic Tool Loading**: Automatically discovers and loads all available MCP tools
- **Semantic Kernel Integration**: Uses Microsoft Semantic Kernel for intelligent conversation handling
- **Flexible AI Provider Support**: Works with both OpenAI and Azure OpenAI
- **Interactive Chat Interface**: Console-based chat experience
- **Robust Error Handling**: Comprehensive error handling and logging throughout
- **Configuration Management**: Environment variable-based configuration for security

## 📋 Prerequisites

- .NET 8.0 SDK or later
- An OpenAI API key OR Azure OpenAI service endpoint
- Access to an MCP server

## ⚙️ Configuration

### 1. **Set Environment Variables**

**For OpenAI:**
```bash
# Windows (PowerShell)
$env:OPENAI_API_KEY="your-openai-api-key-here"

# Windows (Command Prompt)
set OPENAI_API_KEY=your-openai-api-key-here

# Linux/macOS
export OPENAI_API_KEY="your-openai-api-key-here"
```

**For Azure OpenAI:**
```bash
# Windows (PowerShell)
$env:AZURE_OPENAI_API_KEY="your-azure-openai-key"

# Windows (Command Prompt)
set AZURE_OPENAI_API_KEY=your-azure-openai-key

# Linux/macOS
export AZURE_OPENAI_API_KEY="your-azure-openai-key"
```

### 2. **Configure MCP Server and Models (Optional)**

Edit `appsettings.json` to customize:

```json
{
  "SemanticKernel": {
    "OpenAI": {
      "ModelId": "gpt-5-mini"
    },
    "AzureOpenAI": {
      "Endpoint": "https://your-resource.openai.azure.com/",
      "DeploymentName": "gpt-5-mini"
    }
  },
  "McpServer": {
    "Endpoint": "http://your-mcp-server-url/mcp",
    "Name": "LocalHttpClient",
    "ConnectionTimeoutSeconds": 15
  }
}
```

## 🏃‍♂️ Running the Application

### **Method 1: Set environment variable and run**
```bash
# Set the API key
export OPENAI_API_KEY="your-api-key"

# Build and run
dotnet build
dotnet run
```

### **Method 2: Run with inline environment variable**
```bash
# Linux/macOS
OPENAI_API_KEY="your-api-key" dotnet run

# Windows (PowerShell)
$env:OPENAI_API_KEY="your-api-key"; dotnet run
```

### **Method 3: Using .env file (for development)**
Create a `.env` file in the project root:
```
OPENAI_API_KEY=your-api-key-here
```

## 💬 Usage

Once running, you can interact with the AI assistant in natural language:

- **General conversation**: Ask questions or request help
- **Tool discovery**: Ask "what tools are available?" or "list tools"
- **Tool usage**: The AI will automatically use appropriate MCP tools based on your requests
- **Exit**: Type "exit" or "quit" to stop the application

### Example Interactions

```
👤 You: What tools do you have available?
🤖 Assistant: I'll check what MCP tools are available for you...

👤 You: Can you help me analyze this data using your tools?
🤖 Assistant: I'll use the appropriate analysis tools to help you...

👤 You: Please create a report based on the current data
🤖 Assistant: I'll generate a report using the available MCP tools...
```

## 🏗️ Architecture

The application follows a clean, good architecture:

```
Simple.Mcp.Sse.Client/
├── Configuration/          # Configuration models
├── Extensions/            # Dependency injection extensions
├── HostedServices/        # Background services
├── Plugins/              # Semantic Kernel plugins
├── Services/             # Application services
│   ├── IMcpService       # MCP client abstraction
│   ├── ISemanticKernelService # SK abstraction
│   └── IApplicationService    # Main app orchestration
├── Program.cs            # Application entry point
└── appsettings.json     # Configuration file
```

### Key Components

1. **McpService**: Manages connection to MCP server and tool invocation
2. **McpToolsPlugin**: Bridges MCP tools as Semantic Kernel functions
3. **SemanticKernelService**: Handles AI conversations with automatic tool calling
4. **ApplicationService**: Orchestrates the user experience

## 🔧 Customization

### Adding Custom Plugins

You can extend the application by adding custom Semantic Kernel plugins:

```csharp
// In ServiceCollectionExtensions.cs
kernel.Plugins.AddFromType<YourCustomPlugin>("CustomPlugin");
```

### Modifying Tool Behavior

Customize how MCP tools are presented to the kernel by modifying `McpToolsPlugin.cs`.

### Configuration Options

- **Environment Variables**: `OPENAI_API_KEY`, `AZURE_OPENAI_API_KEY`
- **Settings File**: Customize models and MCP server in `appsettings.json`

## 📝 Logging

The application provides structured logging at different levels:

- **Information**: General application flow and successful operations
- **Warning**: Non-critical issues and fallback behaviors
- **Error**: Errors that don't crash the application
- **Debug**: Detailed debugging information (disabled by default)

## 🤝 Error Handling

The application includes comprehensive error handling:

- Connection failures to MCP server are logged and reported
- Tool invocation errors are gracefully handled
- AI service failures include helpful error messages
- Configuration issues are detected at startup

## 📦 Dependencies

- **Microsoft.SemanticKernel**: Core AI orchestration
- **ModelContextProtocol**: MCP client implementation
- **Microsoft.Extensions.Hosting**: Application hosting
- **Microsoft.Extensions.Logging**: Structured logging
- **Microsoft.Extensions.Configuration**: Configuration management

## 🔐 Security Considerations

- ✅ **API keys stored in environment variables** (not in source code)
- ✅ **No sensitive data in configuration files**
- ✅ **Secure credential management**
- Monitor and log tool usage for security auditing
- Implement rate limiting for production deployments
- Validate MCP server certificates in production

## 📚 Next Steps

This foundation can be extended with:

- Web API interface for remote access
- Database integration for conversation history
- Multiple MCP server support
- Custom tool authentication
- Metrics and monitoring integration
- Containerization for deployment

## 🚨 Troubleshooting

### "No AI service configuration found" Error
- Ensure `OPENAI_API_KEY` or `AZURE_OPENAI_API_KEY` environment variable is set
- Verify the API key is valid and has sufficient credits/quota

### MCP Connection Issues
- Check that your MCP server is running and accessible
- Verify the endpoint URL in `appsettings.json` is correct
- Check network connectivity and firewall settings
