using System.ComponentModel.DataAnnotations.Schema;
using Initiate.Model;

namespace Initiate.DataAccess;

public class Address
{
    public int AddressId { get; set; }
    public string Country { get; set; }
    public string ProvinceName { get; set; }
}