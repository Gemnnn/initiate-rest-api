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
        var result = await _userRepository.RegisterUser(registrationDto);

        if (result)
        {
            return Ok("User registered successfully.");
        }

        return BadRequest("User registration failed.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDTO userLoginDto)
    {
        var result = await _userRepository.LoginUser(userLoginDto);

        if (result)
        {
            return Ok("Login successful.");
        }

        return Unauthorized("Invalid credentials.");
    }
}
