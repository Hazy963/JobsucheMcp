using System.Text.Json;
using JobsucheMcp.Models;

namespace JobsucheMcp.Services;

/// <summary>
/// Turns the JSON answer of the API into our simple JobSearchResult.
/// Each API version names its fields differently, so each version has its own method.
/// To support a new version: add a ParseVxx method and one line in Parse().
/// </summary>
public class JobResponseParser
{
    private const string JobPageBaseUrl = "https://www.arbeitsagentur.de/jobsuche/jobdetail/";

    public JobSearchResult Parse(string json, string apiVersion)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        string version = apiVersion.Trim().ToLowerInvariant();

        JobSearchResult result;
        if (version == "v4")
        {
            result = ParseV4(root);
        }
        else
        {
            // v6 and anything newer use the v6 layout.
            result = ParseV6(root);
        }

        result.ApiVersion = version;

        if (result.Jobs.Count == 0)
        {
            result.Message = "No jobs found or the answer could not be read. "
                + "Try a broader title, a bigger radius, or set Jobsuche:ReturnRawJson to true to see the raw answer.";
        }

        return result;
    }

    // Old layout: { "maxErgebnisse": 12, "stellenangebote": [ { "beruf", "arbeitgeber", "arbeitsort": { "ort" }, "refnr" } ] }
    private JobSearchResult ParseV4(JsonElement root)
    {
        JobSearchResult result = new JobSearchResult();
        result.TotalResults = JsonHelper.GetNumber(root, "maxErgebnisse");

        if (!root.TryGetProperty("stellenangebote", out JsonElement list) || list.ValueKind != JsonValueKind.Array)
        {
            return result;
        }

        foreach (JsonElement item in list.EnumerateArray())
        {
            JobSummary job = new JobSummary();
            job.Title = JsonHelper.GetText(item, "beruf");
            job.Employer = JsonHelper.GetText(item, "arbeitgeber");
            job.Location = JsonHelper.FindText(item, "ort");
            job.PublishedDate = JsonHelper.GetText(item, "aktuelleVeroeffentlichungsdatum");
            job.ReferenceNumber = JsonHelper.GetText(item, "refnr");
            job.ExternalUrl = JsonHelper.GetText(item, "externeUrl");
            job.JobUrl = BuildJobUrl(job.ReferenceNumber);
            result.Jobs.Add(job);
        }

        if (result.TotalResults == 0)
        {
            result.TotalResults = result.Jobs.Count;
        }

        return result;
    }

    // Newer layout: { "maxErgebnisse": 12, "ergebnisliste": [ { "referenznummer", "stellenangebotsTitel", "firma", "stellenlokationen", ... } ] }
    // Field names checked against a real v6 answer (November 2026). If the API changes, use ReturnRawJson and adjust here.
    private JobSearchResult ParseV6(JsonElement root)
    {
        JobSearchResult result = new JobSearchResult();
        result.TotalResults = JsonHelper.GetNumber(root, "maxErgebnisse");

        if (!root.TryGetProperty("ergebnisliste", out JsonElement list) || list.ValueKind != JsonValueKind.Array)
        {
            return result;
        }

        foreach (JsonElement item in list.EnumerateArray())
        {
            JobSummary job = new JobSummary();
            job.Title = JsonHelper.GetText(item, "stellenangebotsTitel");
            job.Employer = JsonHelper.GetText(item, "firma");
            job.Location = JsonHelper.FindText(item, "ort");
            job.ReferenceNumber = JsonHelper.GetText(item, "referenznummer");
            job.ExternalUrl = JsonHelper.GetText(item, "externeURL");
            job.PublishedDate = JsonHelper.GetNestedText(item, "veroeffentlichungszeitraum", "von");
            job.StartDate = JsonHelper.GetNestedText(item, "eintrittszeitraum", "von");
            job.DistanceKm = JsonHelper.GetNumber(item, "entfernung");

            job.JobUrl = BuildJobUrl(job.ReferenceNumber);
            result.Jobs.Add(job);
        }

        if (result.TotalResults == 0)
        {
            result.TotalResults = result.Jobs.Count;
        }

        return result;
    }

    private string BuildJobUrl(string referenceNumber)
    {
        if (referenceNumber == "")
        {
            return "";
        }

        return JobPageBaseUrl + referenceNumber;
    }
}
