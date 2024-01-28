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

        public async Task<NewsDTO> CreateNews(NewsDTO newsDTO)
        {
            try
            {
                News news = m_mapper.Map<NewsDTO, News>(newsDTO);
                var addedNews = await m_db.News.AddAsync(news);
                await m_db.SaveChangesAsync();

                return m_mapper.Map<News, NewsDTO>(addedNews.Entity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<int> DeleteNews(int id)
        {
            var newsDetail = await m_db.News.FindAsync(id);

            if (newsDetail != null)
            {
                m_db.News.Remove(newsDetail);
                return await m_db.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<IEnumerable<NewsDTO>> GetAllNews()
        {
            try
            {
                IEnumerable<NewsDTO> newsDTOs = m_mapper.Map<IEnumerable<News>, IEnumerable<NewsDTO>>(m_db.News);

                return newsDTOs;
            }
            catch (Exception ex)
            {
                //TODO: Add logger
                return null;
            }
        }

        public async Task<NewsDTO> GetNews(int id)
        {
            try
            {
                News? news = await m_db.News.FirstOrDefaultAsync(n => n.Id == id);

                if (news == null)
                    throw new Exception("News is no found");

                NewsDTO newsDTO = m_mapper.Map<News, NewsDTO>(news);

                return newsDTO;
            }
            catch (Exception e)
            {
                return null;
            }
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
