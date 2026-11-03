using JobsucheMcp.Services;
using JobsucheMcp.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

// Read appsettings.json from the folder of the .exe,
// not from the folder where Copilot happens to start the program.
HostApplicationBuilderSettings hostSettings = new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
};

HostApplicationBuilder builder = Host.CreateApplicationBuilder(hostSettings);

// MCP uses stdout for messages, so all logs must go to stderr.
builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

// Settings from appsettings.json (section "Jobsuche") and environment variables.
builder.Services.Configure<JobsucheSettings>(
    builder.Configuration.GetSection(JobsucheSettings.SectionName));

// Our own services.
builder.Services.AddSingleton(new HttpClient { Timeout = TimeSpan.FromSeconds(30) });
builder.Services.AddSingleton<JobResponseParser>();
builder.Services.AddSingleton<JobsucheClient>();

// MCP server: talks over stdin/stdout, finds every [McpServerTool] in this project.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();


async Task Test(string title, string area, int radiusKm = 25, int maxResult = 3)
{
    var app = builder.Build();
    var client = app.Services.GetRequiredService<JobsucheClient>();
    var result = await client.SearchAsync(title, area, radiusKm, maxResult);
    Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
    await app.RunAsync();
}