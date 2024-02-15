using Initiate.Business;
using Initiate.DataAccess;
using Initiate.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;

    public UserController(IUserRepository userRepository, UserManager<User> userManager)
    {
        _userRepository = userRepository;
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDTO registrationDto)
    {
        try
        {
            var result = await _userRepository.RegisterUser(registrationDto);
            return Ok(new UserResponse(result ? "User registered successfully." : "User registration failed.", result));
        }
        catch (Exception e)
        {
            return Unauthorized(new UserResponse(e.Message, false));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDTO userLoginDto)
    {
        try
        {
            var result = await _userRepository.LoginUser(userLoginDto);
            return Ok(new UserResponse(result ? "Login Success" : "Login Fail", result));
        }
        catch (Exception e)
        {
            return Unauthorized(new UserResponse(e.Message, false));
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] UserDTO userDto)
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var result = await _userRepository.LogoutUser(userDto);
            return Ok(new UserResponse(result ? "Success" : "Fail", result));
        }
        catch (Exception e)
        {
            return Unauthorized(new UserResponse(e.Message, false));
        }
    }
}