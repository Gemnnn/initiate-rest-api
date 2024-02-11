using Initiate.DataAccess;
using Initiate.Model;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Initiate.Business
{
    public class UserRepository : IUserRepository
    {
        private readonly string ClassName = nameof(UserRepository);
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<UserRepository> _logger;
        private readonly ApplicationDbContext _db;


        public UserRepository(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              ILogger<UserRepository> logger,
                              ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
        }

        public async Task<bool> RegisterUser(UserDTO userDto)
        {
            string methodName = nameof(RegisterUser);

            try
            {
                _logger.LogWarning($"Class:{ClassName}, Method: {methodName}, Message: 'Entered' ");

                if (string.IsNullOrWhiteSpace(userDto.Password) || string.IsNullOrWhiteSpace(userDto.Email))
                    throw new Exception($"Invalid user information: Email - '{userDto.Email}',  Password - '{userDto.Password}' ");

                var user = new User { UserName = userDto.Email, Email = userDto.Email };
                var result = await _userManager.CreateAsync(user, userDto.Password);

                _logger.LogWarning($"Class:{ClassName}, Method: {methodName}, Message: 'Exit' ");

                return result.Succeeded;
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Class:{ClassName}, Method: {methodName}, Message: {e.Message}");
                return false;
            }
        }

        public async Task<bool> LoginUser(UserDTO userDto)
        {
            string methodName = nameof(LoginUser);
            _logger.LogInformation($"Class:{ClassName}, Method: {methodName}, Message: 'Entered' ");

            try
            {
                if (string.IsNullOrWhiteSpace(userDto.Password) || string.IsNullOrWhiteSpace(userDto.Email))
                {
                    throw new Exception($"Invalid user information: Email - '{userDto.Email}',  Password - '{userDto.Password}' ");
                }

                var user = await _userManager.FindByNameAsync(userDto.Email);

                if (user == null)
                {
                    _logger.LogWarning($"Class:{ClassName}, Method: {methodName}, Message: 'User not found' ");
                    return false;
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, userDto.Password, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var newUser = await _db.Users.FirstOrDefaultAsync(user => user.UserName == userDto.Email);
                    User? updatedUser = newUser;
                    if (updatedUser != null)
                    {
                        updatedUser.isSignedIn = true;

                        _db.Users.Update(updatedUser);
                        await _db.SaveChangesAsync();
                    }

                    _logger.LogInformation($"Class:{ClassName}, Method: {methodName}, Message: 'Login succeeded, IsSignedIn: {updatedUser.isSignedIn}' ");

                    return updatedUser.isSignedIn;
                }
                else
                {
                    _logger.LogWarning($"Class:{ClassName}, Method: {methodName}, Message: 'Login failed' ");
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Class:{ClassName}, Method: {methodName}, Message: {e.Message}");
                return false;
            }
        }

        public async Task<bool> LogoutUser(UserDTO userDto)
        {
            string methodName = nameof(LogoutUser);
            _logger.LogWarning($"Class:{ClassName}, Method: {methodName}, Message: 'Entered' ");

            try
            {
                var newUser = await _db.Users.FirstOrDefaultAsync(user => user.UserName == userDto.Email);

                User? updatedUser = newUser;
                if (updatedUser != null && updatedUser.isSignedIn == true)
                {
                    updatedUser.isSignedIn = false;

                    _db.Users.Update(updatedUser);
                    await _db.SaveChangesAsync();

                    await _signInManager.SignOutAsync();
                    _logger.LogWarning($"Class:{ClassName}, Method: {methodName}, Message: 'Exit' ");
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Class:{ClassName}, Method: {methodName}, Message: {e.Message}");
                return false;
            }
        }
    }
}
