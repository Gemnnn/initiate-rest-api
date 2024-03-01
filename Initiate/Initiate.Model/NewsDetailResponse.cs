using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiate.Model
{
    public class NewsDetailResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ShortTitle { get; set; }
        public string? SourceUrl { get; set; }
        public string? Author { get; set; }
        public string? PublishedDate { get; set; }
        public string? Content { get; set; }
    }
}
