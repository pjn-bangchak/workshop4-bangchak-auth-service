using BangchakAuthService.Areas.Identity.Data;
using BangchakAuthService.ModelsDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BangchakAuthService.Controllers;

[ApiController]
[Route("api/v1/[controller]/[action]")] // localhost:port/api/v1/Auth
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public AuthController(UserManager<User> userManager) {
        _userManager = userManager;
    }

    // localhost:port/api/v1/Auth/Home
    [HttpGet]
    public IActionResult Home() {
        return Ok(new {message = "Hello Auth"});
    }

    // localhost:port/api/v1/Auth/Register
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto) {
        
        var bcpUser = new User {
            Email = registerDto.Email,
            UserName = registerDto.Email,
            Fullname = registerDto.Fullname
        };
        
        await _userManager.CreateAsync(bcpUser, registerDto.Password);
        return Ok(new {message = "ลงทะเบียนสำเร็จ" });
    }

    // localhost:port/api/v1/Auth/Login
    [HttpPost]
    public IActionResult Login([FromBody] LoginDto loginDto) {
        return Ok(new {message = "Hello Login"});
    }
}
