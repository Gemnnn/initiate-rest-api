namespace Initiate.DataAccess;

public class Keyword
{
    public int KeywordId { get; set; }
    public string? Word { get; set; }
    public ICollection<UserKeyword>? UserKeywords { get; set; }
}