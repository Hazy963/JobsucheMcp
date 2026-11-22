# JobsucheMcp

**Find German job opportunities directly through your AI assistant.**

A lightweight MCP (Model Context Protocol) server built with C# and .NET 8+. It connects AI assistants to the public job search API of the German Federal Employment Agency (Bundesagentur für Arbeit). Tell your assistant what job you want, where, and how far you'd travel, and it searches for you.

https://github.com/user-attachments/assets/bd545560-6457-4327-99ee-cacfb4e8c992

*Demo: GitHub Copilot searching for C# jobs in Stuttgart through JobsucheMcp.*

## Features

- **Natural language job search:** find jobs by title, location and radius.
- **German job market:** listings from the Bundesagentur für Arbeit.
- **Configurable results:** control the maximum number of returned jobs.
- **No personal API key required:** uses the public `jobboerse-jobsuche` API key.
- **Local execution:** runs on your own computer.
- **Version information:** check the application and API versions.

## Example prompts

- "Find C# developer jobs in Ludwigsburg within 25 km."
- "Search for junior .NET developer positions in Stuttgart."
- "Find software engineering jobs in Germany with a 50 km radius."
- "Show me the latest 10 jobs for WPF developers near Stuttgart."

## Available tools

| Tool | Description |
|---|---|
| `search_jobs` | Search job listings by title, area and radius. |
| `get_server_info` | Retrieve application and API version information. |

## Getting started

### 1. Get the server

Either download the prebuilt version from the [v1.0 release](https://github.com/Hazy963/JobsucheMcp/releases/tag/v1.0) and extract the ZIP to a folder of your choice, or build it yourself:

```
dotnet publish -c Release -o C:\Tools\JobsucheMcp
```

### 2. Connect your AI client

**GitHub Copilot**

1. Open GitHub Copilot and go to **Customize**.
2. Click **Add** in the top-right corner and select **MCP Server**.
3. Point it to `JobsucheMcp.exe`.

<img width="702" height="602" alt="Adding the MCP server in GitHub Copilot" src="https://github.com/user-attachments/assets/0d11def1-45a5-4349-8246-63a11ec08f8f" />

4. Call the server in chat by the name you gave it, for example `/jobsucheMcp`:

<img width="846" height="630" alt="Calling the MCP server in chat" src="https://github.com/user-attachments/assets/757d0d9a-41e0-4ca1-8834-4fbfd79a497b" />

**Claude Desktop**

Add this to `claude_desktop_config.json` (adjust the path), then restart Claude Desktop:

```json
{
  "mcpServers": {
    "jobsuche": {
      "command": "C:\\Tools\\JobsucheMcp\\JobsucheMcp.exe"
    }
  }
}
```

## AI client compatibility

JobsucheMcp uses the open Model Context Protocol, but clients differ in how they connect.

| AI client | Connection | Status |
|---|---|---|
| GitHub Copilot | Local STDIO | Tested |
| Claude Desktop | Local STDIO | Should work, not tested yet |
| OpenAI Codex | Local STDIO | Should work, not tested yet |
| ChatGPT | Remote MCP | Needs a remote transport or tunnel (this server uses STDIO) |

## Technology stack

C#, .NET 8+, Model Context Protocol, Bundesagentur für Arbeit Job Search API, STDIO transport.

## Disclaimer

This is an unofficial project and is not affiliated with the Bundesagentur für Arbeit. It uses their public job search API.

## License

MIT, see [LICENSE](LICENSE).
