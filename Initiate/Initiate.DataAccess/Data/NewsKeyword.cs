using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Initiate.DataAccess;

public class NewsKeyword
{
    [Key]
    public int NewsKeywordId { get; set; }
    public int NewsId { get; set; }
    [ForeignKey("NewsId")]
    public News News { get; set; }

    public int KeywordId { get; set; }
    [ForeignKey("KeywordId")]
    public Keyword Keyword { get; set; }
}