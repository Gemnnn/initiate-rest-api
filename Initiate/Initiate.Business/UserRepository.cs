using Initiate.DataAccess;
using Initiate.Model;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Initiate.Business
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> RegisterUser(UserRegistrationDTO userDto)
        {
            var user = new User { UserName = userDto.Email, Email = userDto.Email };
            var result = await _userManager.CreateAsync(user, userDto.Password);



            return result.Succeeded;
        }

        public async Task<User> LoginUser(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user;
            }
            return null; // Login failed
        }
    }
}
