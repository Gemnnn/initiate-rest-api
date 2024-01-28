using System.ComponentModel.DataAnnotations;

namespace Initiate.DataAccess
{
    public class News
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string Source { get; set; }
    }
}