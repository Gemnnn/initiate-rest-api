using Initiate.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Initiate.Business
{
    public interface INewsService
    {
        Task<IEnumerable<NewsResponse>> GetKeywordNews(string keyword,string username);
        Task<IEnumerable<LocationNewsRespone>> GetLocationNews(string location, string username);
        Task Initialize();
        Task UpdateTimer(string user, string date);
    }
}
