using System.ComponentModel.DataAnnotations.Schema;

namespace Initiate.DataAccess;

public class Preference
{
    public int? PreferenceId { get; set; }
    public string? Language { get; set; }
    public string Country { get; set; }
    public string Province { get; set; }
    public string? NewsGenerationTime { get; set; }
    public bool? IsSetPreference { get; set; }
}