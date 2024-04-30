using BangchakAuthService.ModelsDto;
using Microsoft.AspNetCore.Mvc;

namespace BangchakAuthService.Controllers;

[ApiController]
[Route("api/v1/[controller]/[action]")] // localhost:port/api/v1/Auth
public class AuthController : ControllerBase
{
    // localhost:port/api/v1/Auth/Home
    [HttpGet]
    public IActionResult Home() {
        return Ok(new {message = "Hello Auth"});
    }

    // localhost:port/api/v1/Auth/Register
    [HttpPost]
    public IActionResult Register([FromBody] RegisterDto registerDto) {
        return Ok(new {message = registerDto.Fullname });
    }

    // localhost:port/api/v1/Auth/Login
    [HttpPost]
    public IActionResult Login([FromBody] LoginDto loginDto) {
        return Ok(new {message = "Hello Login"});
    }
}
