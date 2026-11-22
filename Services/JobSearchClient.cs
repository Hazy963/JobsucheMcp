using JobsucheMcp.Models;
using JobsucheMcp.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;

namespace JobsucheMcp.Services;

/// <summary>Talks to the Arbeitsagentur API. Builds the address, sends the request, returns the answer.</summary>
public class JobSearchClient
{
    private readonly HttpClient httpClient;
    private readonly JobsucheSettings settings;
    private readonly JobResponseParser parser;

    public JobSearchClient(HttpClient httpClient, IOptions<JobsucheSettings> options, JobResponseParser parser)
    {
        this.httpClient = httpClient;
        settings = options.Value;
        this.parser = parser;
    }

    /// <summary>Search and return a clean, simple result.</summary>
    public async Task<JobSearchResult> SearchAsync(string title, string area, int radiusKm, int maxResults)
    {
        string json = await SearchRawJsonAsync(title, area, radiusKm, maxResults);
        var jobsSearchResult = parser.Parse(json, settings.ApiVersion);
        if (settings.GetJobDetails)
        {
            foreach (var job in jobsSearchResult.Jobs)
            {
                job.JobDetails = await GetJobDetailsRawJsonAsync(job.ReferenceNumber);
            }
        }


        return jobsSearchResult;
    }


    /// <summary>Search and return the unchanged JSON text of the API.</summary>
    public async Task<string> SearchRawJsonAsync(string title, string area, int radiusKm, int maxResults)
    {
        var url = BuildSearchUrl(title, area, radiusKm, maxResults);
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-API-Key", settings.ApiKey);
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("User-Agent", "JobsucheMcp/1.0");

        using HttpResponseMessage response = await httpClient.SendAsync(request);

        var contentAsString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(BuildErrorMessage(response.StatusCode), null, response.StatusCode);
        }

        return contentAsString;
    }


    public async Task<string> GetJobDetailsRawJsonAsync(string referenznummer)
    {
        string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(referenznummer));
        //v4 is hardcoded hier cause job describtion is just supported in old API version
        string url = $"https://rest.arbeitsagentur.de/jobboerse/jobsuche-service/pc/v4/jobdetails/{encoded}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-API-Key", settings.ApiKey);
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("User-Agent", "JobsucheMcp/1.0");
        //Created hiere cause this is just supported in v4 version - short life client
        using var client = new HttpClient();       
        var response = await client.SendAsync(request);
        var jsonAsString = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(jsonAsString);
        var jobDescription = JsonHelper.GetText(json.RootElement, "stellenangebotsBeschreibung");
        if (response.IsSuccessStatusCode == false)
        {
            throw new HttpRequestException(BuildErrorMessage(response.StatusCode), null, response.StatusCode);
        }


        return jobDescription;
    }

    // Example: https://rest.arbeitsagentur.de/jobboerse/jobsuche-service/pc/v6/jobs?was=C%23%20Entwickler&wo=Ludwigsburg&umkreis=25&size=5&page=1
    private string BuildSearchUrl(string title, string area, int radiusKm, int maxResults)
    {
        string baseUrl = settings.BaseUrl.TrimEnd('/');
        string version = settings.ApiVersion.Trim().ToLowerInvariant();

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
            message += "403 often means this ApiVersion ('" + settings.ApiVersion + "') is retired or not allowed. "
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