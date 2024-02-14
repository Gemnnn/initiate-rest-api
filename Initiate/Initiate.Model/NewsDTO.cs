using System.ComponentModel.DataAnnotations;

namespace Initiate.Model
{
    public class NewsDTO
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Source { get; set; }
    }
}
