using System.Net;
using System.Text;
using JobsucheMcp.Models;
using JobsucheMcp.Settings;
using Microsoft.Extensions.Options;

namespace JobsucheMcp.Services;

/// <summary>Talks to the Arbeitsagentur API. Builds the address, sends the request, returns the answer.</summary>
public class JobsucheClient
{
    private readonly HttpClient _httpClient;
    private readonly JobsucheSettings _settings;
    private readonly JobResponseParser _parser;

    public JobsucheClient(HttpClient httpClient, IOptions<JobsucheSettings> options, JobResponseParser parser)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _parser = parser;
    }

    /// <summary>Search and return a clean, simple result.</summary>
    public async Task<JobSearchResult> SearchAsync(string title, string area, int radiusKm, int maxResults)
    {
        string json = await SearchRawJsonAsync(title, area, radiusKm, maxResults);
        return _parser.Parse(json, _settings.ApiVersion);
    }

    /// <summary>Search and return the unchanged JSON text of the API.</summary>
    public async Task<string> SearchRawJsonAsync(string title, string area, int radiusKm, int maxResults)
    {
        string url = BuildSearchUrl(title, area, radiusKm, maxResults);

        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-API-Key", _settings.ApiKey);
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("User-Agent", "JobsucheMcp/1.0");

        using HttpResponseMessage response = await _httpClient.SendAsync(request);

        // The API does not say which text encoding it uses, so we always read UTF-8 (keeps ü, ä, ö, ß correct).
        byte[] bytes = await response.Content.ReadAsByteArrayAsync();
        string body = Encoding.UTF8.GetString(bytes);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(BuildErrorMessage(response.StatusCode), null, response.StatusCode);
        }

        return body;
    }

    // Example: https://rest.arbeitsagentur.de/jobboerse/jobsuche-service/pc/v6/jobs?was=C%23%20Entwickler&wo=Ludwigsburg&umkreis=25&size=5&page=1
    private string BuildSearchUrl(string title, string area, int radiusKm, int maxResults)
    {
        string baseUrl = _settings.BaseUrl.TrimEnd('/');
        string version = _settings.ApiVersion.Trim().ToLowerInvariant();

        List<string> parameters = new List<string>();

        if (title != "")
        {
            parameters.Add("was=" + Uri.EscapeDataString(title));
        }

        if (area != "")
        {
            parameters.Add("wo=" + Uri.EscapeDataString(area));
            parameters.Add("umkreis=" + radiusKm);
        }

        parameters.Add("size=" + maxResults);
        parameters.Add("page=1");

        return baseUrl + "/pc/" + version + "/jobs?" + string.Join("&", parameters);
    }

    private string BuildErrorMessage(HttpStatusCode statusCode)
    {
        string message = "The job API answered with " + (int)statusCode + " " + statusCode + ". ";

        if (statusCode == HttpStatusCode.Forbidden)
        {
            message += "403 often means this ApiVersion ('" + _settings.ApiVersion + "') is retired or not allowed. "
                + "Change Jobsuche:ApiVersion in appsettings.json (for example to v6) and restart. "
                + "A VPN or a blocked network can also cause 403.";
        }
        else if (statusCode == HttpStatusCode.TooManyRequests)
        {
            message += "Too many requests. Wait a few minutes and try again.";
        }
        else if (statusCode == HttpStatusCode.NotFound)
        {
            message += "The address was not found. Check Jobsuche:BaseUrl and Jobsuche:ApiVersion.";
        }

        return message;
    }
}
