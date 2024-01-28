using Initiate.Model;

namespace Initiate.Business
{
    public interface INewsRepository
    {
        public Task<NewsDTO> CreateNews(NewsDTO newsDTO);
        public Task<NewsDTO> UpdateNews(int id, NewsDTO newsDTO);
        public Task<int> DeleteNews(int id);
        public Task<NewsDTO> GetNews(int id);
        public Task<IEnumerable<NewsDTO>> GetAllNews();
    }
}
