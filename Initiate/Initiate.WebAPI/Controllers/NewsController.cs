using Initiate.Business;
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

        public NewsController(INewsRepository news)
        {
            m_newsRepository = news;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsDTO>>> GetAllNews()
        {
            var newsDTOs = await m_newsRepository.GetAllNews();

            if (newsDTOs == null)
            {
                return NotFound();
            }

            return Ok(newsDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NewsDTO>> GetNews(int id)
        {
            var newsDTO = await m_newsRepository.GetNews(id);

            if (newsDTO == null)
            {
                return NotFound();
            }

            return Ok(newsDTO);
        }

        [HttpPost]
        public async Task<ActionResult<NewsDTO>> NewQuote([FromBody] NewsDTO newsDTO)
        {
            NewsDTO newsResult = null;

            try
            {
                if (newsDTO == null)
                    return BadRequest(newsDTO);

                newsResult = await m_newsRepository.CreateNews(newsDTO);

                if (newsResult == null)
                    return BadRequest("Failed to create news");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return CreatedAtAction(nameof(NewQuote), new { id = newsResult.Id }, newsResult);
        }

        [HttpPut("{id}")]
        [Authorize()]
        public async Task<IActionResult> UpdateNews(int id, [FromBody] NewsDTO quoteDto)
        {
            NewsDTO udpatedNews = null; 
            try
            {
                udpatedNews = await m_newsRepository.UpdateNews(id, quoteDto);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return CreatedAtAction(nameof(NewQuote), new { id = udpatedNews.Id }, udpatedNews);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> UpdateNews(int id)
        {
            int result = 0;
            try
            {
                result = await m_newsRepository.DeleteNews(id);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return CreatedAtAction(nameof(NewQuote), new { id = id }, result);
        }
    }
}
