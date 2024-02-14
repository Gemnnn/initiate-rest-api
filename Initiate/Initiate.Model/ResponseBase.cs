
namespace Initiate.Model
{
    public class ResponseBase
    {
        public ResponseBase(string message, bool isSuccess)
        {
            this.message = message;
            this.isSuccess = isSuccess;
        }
        public bool isSuccess { get; set; }
        public string message { get; set; }
    }
}
