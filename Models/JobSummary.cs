namespace JobsucheMcp.Models;

/// <summary>One job from the search result.</summary>
public class JobSummary
{
    public string Title { get; set; } = "";

    public string Employer { get; set; } = "";

    public string Location { get; set; } = "";

    public string PublishedDate { get; set; } = "";

    /// <summary>Earliest start date of the job (if given).</summary>
    public string StartDate { get; set; } = "";

    /// <summary>Distance from the searched area in km (if given).</summary>
    public int DistanceKm { get; set; }

    public string ReferenceNumber { get; set; } = "";

    /// <summary>Link to the job on arbeitsagentur.de.</summary>
    public string JobUrl { get; set; } = "";

    /// <summary>Link to the employer's own application page (if the API gives one).</summary>
    public string ExternalUrl { get; set; } = "";
}
