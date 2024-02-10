using Initiate.Business;
using Initiate.Model;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
    public async Task<IActionResult> Register([FromBody] UserRegistrationDTO registrationDto)
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
        var user = await _userRepository.LoginUser(userLoginDto.Email, userLoginDto.Password);
        if (user != null)
        {
            return Ok("Login successful.");
        }
        return Unauthorized("Invalid credentials.");
    }
}
