namespace JobsucheMcp.Models;

/// <summary>The answer of one search: how many jobs exist and the first few of them.</summary>
public class JobSearchResult
{
    /// <summary>The API version that produced this result, for example "v6".</summary>
    public string ApiVersion { get; set; } = "";

    /// <summary>How many jobs match in total (not only the returned ones).</summary>
    public int TotalResults { get; set; }

    public List<JobSummary> Jobs { get; set; } = new List<JobSummary>();

    /// <summary>Optional hint for the reader, for example when nothing was found.</summary>
    public string Message { get; set; } = "";
}
