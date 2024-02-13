using System.ComponentModel.DataAnnotations.Schema;

namespace Initiate.DataAccess;

public class Preference
{
    public int PreferenceId { get; set; }
    public string Language { get; set; }
    public int AddressId { get; set; }
    [ForeignKey("AddressId")]
    public Address Address { get; set; }
    public DateTime GenerateDate { get; set; }
}