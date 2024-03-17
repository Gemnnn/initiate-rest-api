namespace Initiate.Model;

public class SharedKeywordDTO
{
    public string Requestor { get; set; } // User email who is sharing the keyword
    public string Receiver { get; set; } // User email to whom the keyword is shared
    public string Keyword { get; set; } // The keyword being shared
    public bool? IsAccepted { get; set; } // null: not responded, true: accepted, false: rejected
}