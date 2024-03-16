using Initiate.Business;
using Initiate.Business.Providers;
using Initiate.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Initiate.WebAPI.Controllers
{

    /// <summary>
    /// News controller.
    /// The news controller uses the INewsRepository to get news data from the DB.
    /// All EntityFramework related work is done in the NewsRepository.
    /// 
    /// 
    /// DO NOT use the News object in here.
    /// MUST use a DTO object because it utilizes loose coupling,
    /// 
    /// Get news from database.
    /// SQL <-----------------> NewsRepository(Business) <-------> NewsController(WebAPI) <------------> Front end (Android)
    ///      News(DataAccess)                             NewsDTO                          REST(NewDTO)
    /// Summarize News.
    /// SQL <-----------------> NewsService/AIService(Business) <-------> NewsController(WebAPI) <------------> Front end (Android)
    ///      News(DataAccess)                             NewsDTO                          REST(NewDTO)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private INewsRepository m_newsRepository;
        private INewsService m_newsService;

        public NewsController(INewsRepository news, INewsService newsService)
        {
            m_newsRepository = news;
            m_newsService = newsService;
        }

        [HttpGet("keyword/{username}/{keyword}")]
        public async Task<ActionResult<IEnumerable<NewsResponse>>> GetAllKeywordNews(string username,string keyword)
        {
            try
            {
                var newsList = await m_newsRepository.GetAllKeywordNews(username, keyword);
                var sortedList = newsList.OrderBy(n => DateTime.Parse(n.PublishedDate)).ToList();


                return Ok(sortedList);
            }
            catch (Exception e)
            {
                return  BadRequest(new ResponseBase(e.Message, false));
            }
        }
        
        [HttpGet("location/{username}/{keyword}")]
        public async Task<ActionResult<IEnumerable<NewsResponse>>> GetAllLocationNews(string username)
        {
            try
            {
                var newsList = await m_newsRepository.GetAllLocationNews(username);
                var sortedList = newsList.OrderBy(n => DateTime.Parse(n.PublishedDate)).ToList();

                return Ok(sortedList);
            }
            catch (Exception e)
            {
                return  BadRequest(new ResponseBase(e.Message, false));
            }
        }

        [HttpGet("location/{username}/{id:int}")]
        public async Task<ActionResult<NewsDetailResponse>> GetLocationNews(int id, string username)
        {
            try
            {
                var newsList = await m_newsRepository.GetNews(username,id);
                return Ok(newsList);
            }
            catch (Exception e)
            {
                return  BadRequest(new ResponseBase(e.Message, false));
            }
        }
        
        [HttpGet("keyword/{username}/{id:int}")]
        public async Task<ActionResult<NewsDetailResponse>> GetKeywordNews(int id, string username)
        {
            try
            {
                var newsList = await m_newsRepository.GetNews(username,id);
                return Ok(newsList);
            }
            catch (Exception e)
            {
                return  BadRequest(new ResponseBase(e.Message, false));
            }
        }
        
        [HttpGet("firstKeyword/{username}/{keyword}")]
        public async Task<ActionResult<IEnumerable<NewsResponse>>> GetFirstKeywordNews(string username, string keyword)
        {
            try
            {
                //Requests to summarize news with keyword and username
                var result = await m_newsService.GetKeywordNews(keyword,username);
                
                //Send the news list to the front end.
                return Ok(result.ToList());
            }
            catch (Exception e)
            {
                return  BadRequest(new ResponseBase(e.Message, false));
            }
        }
        
        [HttpGet("firstLocation/{username}/{keyword}")]
        public async Task<ActionResult<IEnumerable<NewsResponse>>> GetFirstLocationNews(string username, string keyword)
        {
            try
            {
                var result = await m_newsService.GetLocationNews(keyword,username);
                return Ok(result.ToList());
            }
            catch (Exception e)
            {
                return  BadRequest(new ResponseBase(e.Message, false));
            }
        }
    }
}
