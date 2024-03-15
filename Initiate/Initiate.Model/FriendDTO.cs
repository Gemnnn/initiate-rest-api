namespace Initiate.Model;

public class FriendDTO
{
    public string? Requestor { get; set; }
    public string? Receiver { get; set; }
    public RequestStatus Status { get; set; }
}