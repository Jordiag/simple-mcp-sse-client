namespace Simple.Mcp.Http.Client.Configuration;

public class SemanticKernelConfiguration
{
    public OpenAIConfiguration OpenAI { get; set; } = new();
    public AzureOpenAIConfiguration AzureOpenAI { get; set; } = new();
}

public class OpenAIConfiguration
{
    public string ModelId { get; set; } = "gpt-5-mini";
}

public class AzureOpenAIConfiguration
{
    public string Endpoint { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = "gpt-5-mini";
}

public class McpServerConfiguration
{
    public string Endpoint { get; set; } = string.Empty;
    public string Name { get; set; } = "LocalHttpClient";
    public int ConnectionTimeoutSeconds { get; set; } = 15;
}