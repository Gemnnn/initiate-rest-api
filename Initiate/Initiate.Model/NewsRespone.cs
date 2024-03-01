using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiate.Model
{
    public class NewsResponse
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? ShortTitle { get; set; }
        public string? PublishedDate { get; set; }
    }
}
