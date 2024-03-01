using Initiate.Business;
using Initiate.Business.Providers;
using Initiate.Business.Repositories;
using Initiate.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Initiate.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordController : ControllerBase
    {
        IKeywordRepository m_keywordRepository;

        public KeywordController(IKeywordRepository keyword)
        {
            m_keywordRepository = keyword;

        }

        [HttpGet("{username}")]
        public async Task<ActionResult<IEnumerable<KeywordResponse>>> GetAllKeywords(string username)
        {
            try
            {
                var keywordDTOs = await m_keywordRepository.GetAllKeyword(username);
                var response = new List<KeywordResponse>();

                foreach (var keyword in keywordDTOs)
                {
                    var keywordResponse = new KeywordResponse()
                    {
                        Keyword = keyword.Word
                    };
                    response.Add(keywordResponse);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseBase(ex.Message, false));
            }

            
        }

        [HttpPost]
        public async Task<ActionResult<ResponseBase>> CreateKeyword([FromBody] KeywordDTO keywordDTO)
        {
            try
            {
                if (keywordDTO == null)
                    return BadRequest(keywordDTO);

                 await m_keywordRepository.CreateKeyword(keywordDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseBase(ex.Message,false));
            }


            var response = new ResponseBase("Keyword created", true);

            return Ok(response);
        }

        [HttpDelete("{username}/{keyword}")]
        public async Task<IActionResult> DeleteKeyword(string username,string keyword)
        {
            try
            {
                await m_keywordRepository.DeleteKeyword(username,keyword);

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseBase(ex.Message, false));
            }

            return Ok(new ResponseBase("Keyword removded", true));
        }
    }
}
