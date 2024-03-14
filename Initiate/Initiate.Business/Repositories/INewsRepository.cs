using Initiate.Model;

namespace Initiate.Business
{
    public interface INewsRepository
    {
        public Task<NewsDetailResponse> GetNews(string username, int id);
        public Task<IEnumerable<NewsResponse>> GetAllKeywordNews(string username,string keyword);
        public Task<IEnumerable<LocationNewsRespone>> GetAllLocationNews(string username);
    }
}
