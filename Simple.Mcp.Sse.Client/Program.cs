using ModelContextProtocol.Client;


namespace Simple.Mcp.Sse.Client
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            const string McpServerUrl = "https://example.com/mcp"; // Replace with your MCP server URL

            var options = new SseClientTransportOptions
            {
                Endpoint = new Uri(McpServerUrl),
                Name = "LocalHttpClient",
                ConnectionTimeout = TimeSpan.FromSeconds(15)
            };

            var transport = new SseClientTransport(options);
            IMcpClient client = await McpClientFactory.CreateAsync(transport);

            Console.WriteLine("✅ Connected to MCP server over SSE HTTP");

            IList<McpClientTool> tools = await client.ListToolsAsync();

            Console.WriteLine("\n🛠️ Available Tools:");


            if (tools.Count > 0)
            {
                foreach (McpClientTool tool in tools)
                    Console.WriteLine($" - {tool.Name}: {tool.Description}");
            }

            await client.DisposeAsync();
        }
    }
}
