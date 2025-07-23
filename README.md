# 🧠 Simple.Mcp.Sse.Client

A lightweight C# client that connects to a Model Context Protocol (MCP) server using **Server-Sent Events (SSE)** over HTTP. This example demonstrates how to initialise a client, establish a connection to a local MCP server, and list available tools.

---

## 🚀 Features

- ✅ Connects to an MCP server using SSE
- 🔒 Configurable connection timeout
- 🛠️ Lists all tools provided by the MCP server
- 🧼 Clean and minimal async code structure

---

## 📦 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- An MCP server running on `http://{domain}:{port}`

---

## 🛠️ Usage

1. **Clone the repo:**
   ```bash
   git clone https://github.com/your-username/Simple.Mcp.Sse.Client.git
   cd Simple.Mcp.Sse.Client

2. **Build and run**:
	```bash
	dotnet run

3. **Output Example**:
	```bash
	 ✅ Connected to MCP server over SSE HTTP

 	 🛠️ Available Tools:
	  - ToolName1: Description of Tool 1
	  - ToolName2: Description of Tool 2


## 📁 Project Structure

	Simple.Mcp.Sse.Client/
	├── Program.cs       # Main entry point
	├── README.md        # You're reading it!
	└── ...              # Other dependencies or files

## 🤝 Contributing
Feel free to fork this repository, submit issues, or open pull requests to contribute.

## 📄 License
This project is licensed under the MIT License.
