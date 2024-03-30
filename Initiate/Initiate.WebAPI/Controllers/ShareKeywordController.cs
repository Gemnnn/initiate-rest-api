using Initiate.Business.Repositories;
using Initiate.Model;
using Microsoft.AspNetCore.Mvc;

namespace Initiate.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShareKeywordController : Controller
{
    private readonly ISharedKeywordRepository _sharedKeywordRepository;

    public ShareKeywordController(ISharedKeywordRepository sharedKeywordRepository)
    {
        _sharedKeywordRepository = sharedKeywordRepository;
    }

    // GET: api/sharekeyword/{username}
    [HttpGet("{username}")]
    public async Task<IActionResult> GetSharedKeywords(string username)
    {
        // Logic to retrieve shared keywords for the user
        var sharedKeywords = await _sharedKeywordRepository.GetSharedKeywordsAsync(username);

        // Extract keyword + the requestor's username
        // No logic neccessary for empty/null list -> Can just display empty list
        var keywords = sharedKeywords.Select(sk => new
        {
            sk.Keyword,
            sk.Requestor,
            sk.Receiver
        }).ToList();
        
        return Ok(keywords);
    }

    // POST: api/sharekeyword
    [HttpPost]
    public async Task<IActionResult> ShareKeyword([FromBody] ShareKeywordRequest shareKeywordRequest)
    {
        try
        {
            var sharedKeywordDTO = new SharedKeywordDTO
            {
                Requestor = shareKeywordRequest.Requestor,
                Receiver = shareKeywordRequest.Receiver,
                Keyword = shareKeywordRequest.Keyword,
                IsAccepted = null
            };

            await _sharedKeywordRepository.ShareKeywordAsync(sharedKeywordDTO);
            return Ok(new { isSuccess = true, message = "Keyword shared successfully" });
        }
        catch (Exception e)
        {
            return BadRequest(new { isSuccess = true, message = e.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> AcceptOrRejectKeyword([FromBody] AcceptRejectRequest acceptRejectRequest)
    {
        var sharedKeywordDTO = new SharedKeywordDTO
        {
            Receiver = acceptRejectRequest.Username,
            Keyword = acceptRejectRequest.Keyword,
            IsAccepted = acceptRejectRequest.Accept
        };

        await _sharedKeywordRepository.AcceptOrRejectKeywordAsync(sharedKeywordDTO);
        return Ok(new { isSuccess = true, message = "Keyword response updated successfully" });
    }


    public class AcceptRejectRequest
    {
        public string Username { get; set; }
        public string Keyword { get; set; }
        public bool Accept { get; set; }
    }

    public class ShareKeywordRequest
    {
        public string Requestor { get; set; }
        public string Receiver { get; set; }
        public string Keyword { get; set; }
    }
}