namespace Initiate.Model;

public class FriendResponse
{
    public string? FriendUsername { get; set; }
    public int? Status { get; set; }
    public bool? IsRequestedByCurrentUser { get; set; }
}