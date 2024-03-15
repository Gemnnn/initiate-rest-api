
using Initiate.DataAccess;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Initiate.Model
{
    public class Friend
    {
        [Key]
        public int FriendId { get; set; } 

        [ForeignKey("Requester")]
        public string RequesterId { get; set; } 
        public virtual User Requester { get; set; } 

        [ForeignKey("Receiver")]
        public string ReceiverId { get; set; }
        public virtual User Receiver { get; set; }

        public RequestStatus Status { get; set; }
    }

    public enum RequestStatus
    {
        Pending = 1, 
        Accepted,
    }
}
