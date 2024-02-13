using Initiate.Model;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Initiate.DataAccess
{
    public class User : IdentityUser
    {
        // Additional properties can be defined here.
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int PreferenceId { get; set; }
        public bool isSignedIn { get; set; }
        [ForeignKey("PreferenceId")]
        public Preference Preference { get; set; }
        public ICollection<UserKeyword> UserKeywords { get; set; }
        public virtual ICollection<Friend> RequestedFriends { get; set; }
        public virtual ICollection<Friend> ReceivedFriends { get; set; }
    }
}
