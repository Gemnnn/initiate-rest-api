using AutoMapper;
using Initiate.DataAccess;
using Initiate.Model;
using Microsoft.EntityFrameworkCore;

namespace Initiate.Business
{
    /// <summary>
    /// News repository to connect with db for news
    /// </summary>
    public class NewsRepository : INewsRepository
    {
        private readonly ApplicationDbContext m_db;
        private readonly IMapper m_mapper;

        public NewsRepository(ApplicationDbContext db, IMapper mapper)
        {
            m_db = db;
            m_mapper = mapper;
        }

        public async Task<IEnumerable<NewsResponse>> GetAllKeywordNews(string username, string keyword)
        {
            var user = await m_db.Users.Include(x => x.News).FirstOrDefaultAsync(x => x.UserName == username);

            var news = user.News.Where(x => x.Keyword.ToLower() == keyword.ToLower());
            var newsResponses = news.Select(x => new NewsResponse()
                {
                    Id = x.NewsId,
                    Title = x.Title,
                    ShortTitle = x.ShortTitle,
                    PublishedDate = x.PublishedDate.ToString("MMM dd, yyyy hh:mm:ss tt")
                }
            );

            return newsResponses;
        }

        public async Task<IEnumerable<NewsResponse>> GetAllLocationNews(string username)
        {
            var user = await m_db.Users.Include(x => x.News).FirstOrDefaultAsync(x => x.UserName == username);

            var news = user.News.Where(x => x.IsLocation == true);
            var newsResponses = news.Select(x => new NewsResponse()
                {
                    Id = x.NewsId,
                    Title = x.Title,
                    ShortTitle = x.ShortTitle,
                    PublishedDate = x.PublishedDate.ToString("HH:mm:ss")
                }
            );
            return newsResponses;
        }

        public async Task<NewsDetailResponse> GetNews(string username, int id)
        {
            News? news = await m_db.News.FirstOrDefaultAsync(n => n.NewsId == id);

            if (news == null)
                throw new Exception("News is no found");

            NewsDetailResponse newsDetailResponse = new NewsDetailResponse()
            {
                Id = news.NewsId,
                Title = news.Title,
                ShortTitle = news.ShortTitle,
                SourceUrl = news.Source,
                Author = news.Author,
                PublishedDate = news.PublishedDate.ToString("HH:mm:ss"),
                Content = news.Content
            };

            return newsDetailResponse;
        }

        public async Task<NewsDTO> UpdateNews(int id, NewsDTO newsDTO)
        {
            try
            {
                if (id == newsDTO.Id)
                {
                    News? newsDetail = await m_db.News.FindAsync(id);

                    if (newsDetail == null)
                        throw new Exception("News is no found");


                    News? news = m_mapper.Map<NewsDTO, News>(newsDTO, newsDetail);
                    var updatedNews = m_db.News.Update(news);
                    await m_db.SaveChangesAsync();

                    return m_mapper.Map<News, NewsDTO>(updatedNews.Entity);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}