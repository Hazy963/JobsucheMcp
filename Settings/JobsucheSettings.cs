namespace JobsucheMcp.Settings;

/// <summary>
/// All values that can be changed in appsettings.json (section "Jobsuche")
/// or with environment variables like Jobsuche__ApiVersion.
/// </summary>
public class JobsucheSettings
{
    public const string SectionName = "Jobsuche";

    /// <summary>Base address of the Arbeitsagentur job service.</summary>
    public string BaseUrl { get; set; } = "https://rest.arbeitsagentur.de/jobboerse/jobsuche-service";

    /// <summary>Public key of the Arbeitsagentur API (no personal account needed).</summary>
    public string ApiKey { get; set; } = "jobboerse-jobsuche";

    /// <summary>
    /// Version of the search endpoint, for example "v6" or "v4".
    /// The address becomes: BaseUrl/pc/{ApiVersion}/jobs
    /// </summary>
    public string ApiVersion { get; set; } = "v6";

    /// <summary>Radius in km when the caller does not give one.</summary>
    public int DefaultRadiusKm { get; set; } = 25;

    /// <summary>Number of jobs when the caller does not give a number.</summary>
    public int DefaultMaxResults { get; set; } = 5;

    /// <summary>Upper limit for the number of jobs.</summary>
    public int MaxAllowedResults { get; set; } = 50;

    /// <summary>
    /// When true, the tool returns the unchanged API answer.
    /// Useful to see the real field names if the API changes.
    /// </summary>
    public bool ReturnRawJson { get; set; } = false;
}
