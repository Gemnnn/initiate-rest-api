using System.ComponentModel.DataAnnotations;

namespace Initiate.Model
{
    public class NewsDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter news title")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Please enter news content")]
        public string? Content { get; set; }

        public string? Source { get; set; }
    }
}
