using System.ComponentModel;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using JobsucheMcp.Models;
using JobsucheMcp.Services;
using JobsucheMcp.Settings;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;

namespace JobsucheMcp.Tools;

/// <summary>The tools that Copilot can call. Every method with [McpServerTool] becomes a tool.</summary>
[McpServerToolType]
public class JobSearchTools
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        // Keep umlauts readable (ü, ä, ö) instead of \u00FC
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private readonly JobSearchClient _client;
    private readonly JobsucheSettings _settings;

    public JobSearchTools(JobSearchClient client, IOptions<JobsucheSettings> options)
    {
        _client = client;
        _settings = options.Value;
    }

    [McpServerTool(Name = "search_jobs")]
    [Description("Search job offers in Germany (Bundesagentur für Arbeit) by job title and area, with a radius in km.")]
    public async Task<string> SearchJobs(
        [Description("Job title or keywords, for example 'C# Entwickler' or '.NET Software Developer'.")] string title,
        [Description("City or area, for example 'Ludwigsburg'.")] string area,
        [Description("Search radius around the area in km, for example 10, 25, 50 or 100. Optional.")] int? radiusKm = null,
        [Description("How many jobs to return. Optional.")] int? maxResults = null)
    {
        string cleanTitle = (title ?? "").Trim();
        string cleanArea = (area ?? "").Trim();

        int radius = radiusKm ?? _settings.DefaultRadiusKm;
        if (radius < 0)
        {
            radius = 0;
        }

        int count = maxResults ?? _settings.DefaultMaxResults;
        if (count < 1)
        {
            count = 1;
        }

        if (count > _settings.MaxAllowedResults)
        {
            count = _settings.MaxAllowedResults;
        }

        try
        {
            if (_settings.ReturnRawJson)
            {
                return await _client.SearchRawJsonAsync(cleanTitle, cleanArea, radius, count);
            }

            JobSearchResult result = await _client.SearchAsync(cleanTitle, cleanArea, radius, count);
            return JsonSerializer.Serialize(result, JsonOptions);
        }
        catch (HttpRequestException exception)
        {
            return "Job search failed: " + exception.Message;
        }
    }

    [McpServerTool(Name = "get_server_info")]
    [Description("Shows the app version and the job API version this server is using.")]
    public string GetServerInfo()
    {
        Version? appVersion = Assembly.GetExecutingAssembly().GetName().Version;

        string info = "App version: " + (appVersion?.ToString(3) ?? "unknown") + "\n"
            + "API version: " + _settings.ApiVersion + "\n"
            + "API base address: " + _settings.BaseUrl + "\n"
            + "Raw JSON mode: " + _settings.ReturnRawJson;

        return info;
    }
}
