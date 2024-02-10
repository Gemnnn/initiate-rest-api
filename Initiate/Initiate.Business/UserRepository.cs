using Initiate.DataAccess;
using Initiate.Model;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
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

        public async Task<bool> RegisterUser(UserDTO userDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userDto.Password) || string.IsNullOrWhiteSpace(userDto.Email))
                    throw new Exception($"Invalid user information: Email - '{userDto.Email}',  Password - '{userDto.Password}' ");

                var user = new User { UserName = userDto.Email, Email = userDto.Email };
                var result = await _userManager.CreateAsync(user, userDto.Password);
                return result.Succeeded;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> LoginUser(UserDTO userDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userDto.Password) || string.IsNullOrWhiteSpace(userDto.Email))
                    throw new Exception($"Invalid user information: Email - '{userDto.Email}',  Password - '{userDto.Password}' ");

                var user = await _userManager.FindByNameAsync(userDto.Email);
                
                if (user == null)
                    return false;

                var result = await _signInManager.CheckPasswordSignInAsync(user, userDto.Password, lockoutOnFailure: false);

                return result.Succeeded;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
