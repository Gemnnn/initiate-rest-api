using Microsoft.AspNetCore.Identity;

namespace Initiate.DataAccess
{
    public class User : IdentityUser
    {
        // Additional properties can be defined here.
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
