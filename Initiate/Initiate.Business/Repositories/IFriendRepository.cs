using Initiate.Model;

namespace Initiate.Business.Repositories;

public interface IFriendRepository
{
    public Task<IEnumerable<FriendDTO>> GetAllFriends(string username);
    public Task RequestFriend(FriendDTO friendDto);
    public Task RejectFriend(FriendDTO friendDto);
    public Task AcceptFriend(FriendDTO friendDto);
}