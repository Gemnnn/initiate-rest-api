using Initiate.Business.Repositories;
using Initiate.Model;
using Microsoft.AspNetCore.Mvc;

namespace Initiate.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        IFriendRepository m_friendRepository;

        public FriendController(IFriendRepository friendRepository)
        {
            m_friendRepository = friendRepository;

        }

        [HttpGet("{username}")]
        public async Task<ActionResult<IEnumerable<FriendResponse>>> GetAllFriends(string username)
        {
            try
            {
                var friends = await m_friendRepository.GetAllFriends(username);

          
                var friendResponses = friends.Select(f => new FriendResponse
                {
                    FriendUsername = f.Requestor == username ? f.Receiver : f.Requestor,
                    Status = (int)f.Status,
                    IsRequestedByCurrentUser = f.Requestor == username
                });

                return Ok(friendResponses);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseBase(ex.Message, false));
            }

            
        }

        [HttpPost("{username}")]
        public async Task<ActionResult<ResponseBase>> RequestFriend(string username, [FromBody] FriendRequest friend)
        {
            try
            {
                var friendDTO = new FriendDTO();
                friendDTO.Requestor = username;
                friendDTO.Receiver = friend.FriendUsername;
                friendDTO.Status = RequestStatus.Pending;
                
                await m_friendRepository.RequestFriend(friendDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseBase(ex.Message,false));
            }
            
            return Ok(new ResponseBase("Requested friend",true));
        }
        
        [HttpPut("{username}")]
        public async Task<ActionResult<ResponseBase>> AcceptFriend(string username, [FromBody] FriendRequest friend)
        {
            try
            {
                var friendDTO = new FriendDTO
                {
                    Requestor = friend.FriendUsername, 
                    Receiver = username,
                    Status = RequestStatus.Pending 
                };

                await m_friendRepository.AcceptFriend(friendDTO);

                return Ok(new ResponseBase("Friend request accepted.", true));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseBase(ex.Message, false));
            }
        }

        [HttpDelete("{username}")]
        public async Task<ActionResult<ResponseBase>> RejectFriend(string username, [FromBody] FriendRequest friend)
        {
            try
            {
                var friendDTO = new FriendDTO
                {
                    Requestor = friend.FriendUsername, 
                    Receiver = username,
                    Status = RequestStatus.Pending
                };

                await m_friendRepository.RejectFriend(friendDTO);

                return Ok(new ResponseBase("Friend request rejected.", true));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseBase(ex.Message, false));
            }
        }
    }
}
