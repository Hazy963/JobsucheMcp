# JobsucheMcp
A lightweight C#/.NET MCP server for searching German job listings from the Bundesagentur für Arbeit. Compatible with MCP-enabled AI clients such as ChatGPT, Claude Desktop, and GitHub Copilot.


# JobsucheMcp

**Find German job opportunities directly through your AI assistant.**

JobsucheMcp is a lightweight MCP (Model Context Protocol) server built with C# and .NET 8+. It connects AI assistants to the public job search API of the German Federal Employment Agency (Bundesagentur für Arbeit).

Search German job listings using natural language. Simply tell your AI assistant what kind of job you're looking for, where you want to work, and how far you're willing to travel.

## AI Client Compatibility

JobsucheMcp uses the open Model Context Protocol (MCP), allowing different AI clients to interact with the same job-search tools.

| AI Client | Support |
|---|---|
| GitHub Copilot | Yes – local stdio |
| Claude Desktop | Yes – local stdio |
| ChatGPT | Yes – with a supported remote MCP connection |
| OpenAI Codex | Yes – local stdio |

**Important:** This implementation currently uses the STDIO transport. ChatGPT requires a remote MCP connection for custom MCP servers, so additional transport configuration or a suitable tunnel is needed. MCP compatibility does not mean every client supports the same connection method.

## Features

- **Natural language job search:** Ask your AI assistant to find jobs by title, location and radius.
- **German job market:** Search listings from the Bundesagentur für Arbeit.
- **Location-based search:** Specify an area and search radius in kilometers.
- **Configurable results:** Control the maximum number of returned jobs.
- **No personal API key required:** Uses the public `jobboerse-jobsuche` API key.
- **Local execution:** Runs on your own computer using .NET.
- **Version information:** Check the application and API versions.

## Getting Started
- Build your Release ```dotnet publish -c Release -o C:\Tools\JobsucheMcp``` Folder
- or dwonload prebuilt one frm releases:
- https://github.com/Hazy963o/JobsucheMcp/releases/tag/v1.0
- Extract the ZIP file to a folder of your choice.
- Point your MCP client to JobsucheMcp.exe:
- Open GitHub Copilot and navigate to Customize.
- In the top-right corner, click Add.
- Select MCP Server
- <img width="702" height="602" alt="image" src="https://github.com/user-attachments/assets/0d11def1-45a5-4349-8246-63a11ec08f8f" />
- call your mcp server as you named it in your configuration with "/<McpName>":
- <img width="846" height="630" alt="image" src="https://github.com/user-attachments/assets/757d0d9a-41e0-4ca1-8834-4fbfd79a497b" />


## Full github copilot Integration + Result video demo 
https://github.com/user-attachments/assets/bd545560-6457-4327-99ee-cacfb4e8c992



## Available MCP Tools

| Tool | Description |
|---|---|
| `search_jobs` | Search job listings by title, area and radius. |
| `get_server_info` | Retrieve application and API version information. |

## Example Prompts

Ask your AI assistant:

- "Find C# developer jobs in Ludwigsburg within 25 km."
- "Search for junior .NET developer positions in Stuttgart."
- "Find software engineering jobs in Germany with a 50 km radius."
- "Show me the latest 10 jobs for WPF developers near Stuttgart."

## Technology Stack

- C#
- .NET 8+
- Model Context Protocol (MCP)
- Bundesagentur für Arbeit Job Search API
- STDIO transport






