using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Initiate.DataAccess;
using Microsoft.EntityFrameworkCore;

public class UserKeyword
{
    [Key]
    public int UserKeywordId { get; set; }
    public string UserId { get; set; } // Changed from int to string
    [ForeignKey("UserId")]
    public User User { get; set; }

    public int KeywordId { get; set; }
    [ForeignKey("KeywordId")]
    public Keyword Keyword { get; set; }
}
