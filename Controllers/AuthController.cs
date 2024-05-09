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
    private readonly SignInManager<User> _signInManager;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager) {
        _userManager = userManager;
        _signInManager = signInManager;
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
        
        var result = await _userManager.CreateAsync(bcpUser, registerDto.Password);

        if (!result.Succeeded) {
            return BadRequest(result.Errors);
        }

        return Ok(new {message = "ลงทะเบียนสำเร็จ" });
    }

    // localhost:port/api/v1/Auth/Login
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto) {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) {
            return NotFound();
        }

        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
        if (!result.Succeeded) {
            return Unauthorized(new {message = "รหัสผ่านไม่ถูกต้อง"});
        }

        return Ok(new {message = "เข้าระบบสำเร็จ แล้วสร้าง token"});
    }
}
