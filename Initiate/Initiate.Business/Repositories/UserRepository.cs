using Initiate.DataAccess;
using Initiate.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Initiate.Common;

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

                var preference = new Preference()
                {
                    Country = string.Empty,
                    Province = string.Empty,
                    NewsGenerationTime = DateTime.Now.ToString("HH:mm"),
                    Language = string.Empty,
                    IsSetPreference = false
                };

                await _db.Preferences.AddAsync(preference);
                await _db.SaveChangesAsync();

                var newPreference = await _db.Preferences.FirstOrDefaultAsync(x=>x.NewsGenerationTime == preference.NewsGenerationTime);

                var user = new User { UserName = userDto.Email, Email = userDto.Email, PreferenceId = newPreference?.PreferenceId??0 };
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
                await CreateUserAsync(userDto);

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

                var result = await _signInManager.PasswordSignInAsync(userDto.Email, userDto.Password, false, lockoutOnFailure: false);

                _logger.LogInformation($"Class:{ClassName}, Method: {methodName}, Message: 'Login Status': {result.Succeeded}' ");

                return result.Succeeded;

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

        private async Task CreateUserAsync(UserDTO userDTO)
        {
            try
            {
                var username = userDTO?.Email;
                var password = userDTO?.Password;

                if (!TestUsers.Users.ContainsKey(username))
                    return;

                var defaultUser = await _db.Users.FirstOrDefaultAsync(x => x.UserName == username);

                if (defaultUser != null)
                    return;

                var user = new UserDTO
                {
                    Email = username,
                    Password = password,
                };

                await RegisterUser(user);

                return;
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
