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
    /// Basic Structure.
    /// SQL <-----------------> NewsRepository(Business) <-------> NewsController(WebAPI) <------------> Front end (Android)
    ///      News(DataAccess)                             NewsDTO                          REST(NewDTO)
    ///    
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        INewsRepository m_newsRepository;
        INewsProvider m_newsProvider;
        IChatGPTProvider m_chatGPTProvider;

        public NewsController(INewsRepository news, INewsProvider newsProvider, IChatGPTProvider chatGPTProvider)
        {
            m_newsRepository = news;
            m_newsProvider = newsProvider;
            m_chatGPTProvider = chatGPTProvider;
        }

        [HttpGet("{username}/{keyword}")]
        public async Task<ActionResult<IEnumerable<NewsResponse>>> GetAllNews(string keyword)
        {
            //var newsDTOs = await m_newsRepository.GetAllNews();

            //if (newsDTOs == null)
            //{
            //    return NotFound();
            //}

            //await m_newsProvider.GetNews();
            //await m_chatGPTProvider.GetSummerizeedNews();

            //return Ok(newsDTOs);


            var newsList = new List<NewsResponse>();

            newsList.Add(new NewsResponse()
            {
                Id = 1,
                Title = "Test Title 1",
                PublishedDate = DateTime.Now.ToString()
            }); 

            newsList.Add(new NewsResponse()
            {
                Id = 2,
                Title = "Test Title 2",
                PublishedDate = DateTime.Now.ToString()
            });

            return newsList; 
        }

        [HttpGet("{username}/{id:int}")]
        public async Task<ActionResult<NewsDetailResponse>> GetNews(int id, string username)
        {
            //var newsDTO = await _newsRepository.GetNews(id, category);

            //if (newsDTO == null)
            //{
            //    return NotFound();
            //}

            //return Ok(newsDTO);

            var response = new NewsDetailResponse()
            {
                Id = id,
                Title = $"Test Title + {id}",
                Content = $"Test Content + {id} + {username}",
            };


            return Ok(response);
        }
    }
}
