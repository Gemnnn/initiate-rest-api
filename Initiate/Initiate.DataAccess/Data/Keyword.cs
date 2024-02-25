namespace Initiate.DataAccess;

public class Keyword
{
    public int KeywordId { get; set; }
    public int Word { get; set; }
    //public ICollection<NewsKeyword> NewKeyword { get; set; }
    public ICollection<UserKeyword> UserKeywords { get; set; }
}