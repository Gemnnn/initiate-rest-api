using System.ComponentModel.DataAnnotations;

namespace Initiate.DataAccess
{
    public class News
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Author { get; set; }
        public string Source { get; set; }
        public string Desciprtion { get; set; }
        public ICollection<NewsKeyword> NewsKeywords { get; set; }
        public string Content { get; set; }
    }
}