using System.ComponentModel.DataAnnotations.Schema;

namespace Initiate.DataAccess.Data;

public class SharedKeyword
{
    public int SharedKeywordId { get; set; }
    public string Sender { get; set; }
    public string Receiver { get; set; }
    public string Keyword { get; set; }
    public bool? IsAccepted { get; set; }
}