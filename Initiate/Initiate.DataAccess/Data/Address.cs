using System.ComponentModel.DataAnnotations.Schema;
using Initiate.Model;

namespace Initiate.DataAccess;

public class Address
{
    public int AddressId { get; set; }
    public int CountryId { get; set; }
    [ForeignKey("CountryId")]
    public Country Country { get; set; }
    public int ProvinceId { get; set; }
    [ForeignKey("ProvinceId")]
    public Province Province { get; set; }
}