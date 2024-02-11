namespace Initiate.Model;

public class UserResponse
{
    public UserResponse(string message, bool isSuccess)
    {
        this.message = message;
        this.isSuccess = isSuccess;
    }
    public bool isSuccess { get; set; }
    public string message { get; set; }
}