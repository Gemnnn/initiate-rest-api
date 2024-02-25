using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Initiate.Model;

public class PreferenceDTO : RequestBase
{
    /// <summary>
    /// Indicates the language of the user
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Indicates the province of the user
    /// </summary>
    public string? Province { get; set; }

    /// <summary>
    /// Indicates the country of the user
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Indicates the news generation time of the user (HH:MM:SS)
    /// </summary>
    public string? NewsGenerationTime { get; set; }

    /// <summary>
    /// Indicates the Email of the user
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Indicates the preference of the user
    /// </summary>
    public bool? IsSetPreference { get; set; } = false;
}