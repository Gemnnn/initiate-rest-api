using Initiate.DataAccess;
using Initiate.Model;
using Microsoft.EntityFrameworkCore;

namespace Initiate.Business.Repositories;

public class FriendRepository : IFriendRepository
{
    private readonly ApplicationDbContext m_db;

    public FriendRepository(ApplicationDbContext db)
    {
        m_db = db;
    }
    
    public async Task<IEnumerable<FriendDTO>> GetAllFriends(string username)
    {
        var friends = await m_db.Friends
            .Where(f => (f.Requester.UserName == username || f.Receiver.UserName == username))
            .Select(f => new FriendDTO
            {
                Requestor = f.Requester.UserName,
                Receiver = f.Receiver.UserName,
                Status = f.Status
            })
            .ToListAsync();
        
        return friends;
    }

    public async Task RequestFriend(FriendDTO friendDto)
    {
        var requester = await m_db.Users.FirstOrDefaultAsync(x => x.UserName == friendDto.Requestor);
        var receiver = await m_db.Users.FirstOrDefaultAsync(x => x.UserName == friendDto.Receiver);

        if (requester == null || receiver == null)
            throw new Exception("Requestor or Receiver does not exist.");

        // Check if the friend request already exists
        var friendRequestExists = await m_db.Friends.AnyAsync(
            f => (f.RequesterId == requester.Id && f.ReceiverId == receiver.Id) ||
                 (f.RequesterId == receiver.Id && f.ReceiverId == requester.Id));
    
        if (friendRequestExists)
            throw new Exception("Friend request already exists.");

        // Create a new friend request
        var friendRequest = new Friend
        {
            RequesterId = requester.Id,
            ReceiverId = receiver.Id,
            Status = RequestStatus.Pending
        };

        await m_db.Friends.AddAsync(friendRequest);
        await m_db.SaveChangesAsync();
    }

    public async Task RejectFriend(FriendDTO friendDto)
    {
        var friendRequest = await m_db.Friends
            .FirstOrDefaultAsync(f => (f.Requester.UserName == friendDto.Requestor && f.Receiver.UserName == friendDto.Receiver) ||
                                      (f.Requester.UserName == friendDto.Receiver && f.Receiver.UserName == friendDto.Requestor));
        
        if (friendRequest == null)
            throw new Exception("Friend request not found.");

        m_db.Friends.Remove(friendRequest);
        await m_db.SaveChangesAsync();
    }

    public async Task AcceptFriend(FriendDTO friendDto)
    {
        if (friendDto == null)
            throw new ArgumentNullException(nameof(friendDto));

        var friendRequest = await m_db.Friends
            .Include(f => f.Requester) // Ensure related data is included
            .Include(f => f.Receiver)
            .FirstOrDefaultAsync(f => (f.Requester.UserName == friendDto.Requestor && f.Receiver.UserName == friendDto.Receiver) ||
                                      (f.Requester.UserName == friendDto.Receiver && f.Receiver.UserName == friendDto.Requestor));
        
        if (friendRequest == null)
            throw new Exception("Friend request not found.");
        
        if (friendRequest.Receiver == null || friendRequest.Requester == null)
            throw new InvalidOperationException("Invalid friend request state.");
        
        if (friendRequest.Receiver.UserName != friendDto.Receiver)
            throw new Exception("Only the receiver can accept the friend request.");
        
        friendRequest.Status = RequestStatus.Accepted;
        
        m_db.Friends.Update(friendRequest);
        await m_db.SaveChangesAsync();
    }

}