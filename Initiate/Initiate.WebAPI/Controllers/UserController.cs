using Initiate.Business;
using Initiate.Model;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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
            return Ok(new UserResponse(e.Message, false));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDTO userLoginDto)
    {
        try
        {
            var result = await _userRepository.LoginUser(userLoginDto);
            return Ok(new UserResponse(result ? "Success" : "Fail", result));
        }
        catch (Exception e)
        {
            return Ok(new UserResponse(e.Message, false));
        }
    }
}