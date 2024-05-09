using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BangchakAuthService.Areas.Identity.Data;
using BangchakAuthService.ModelsDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace BangchakAuthService.Controllers;

[ApiController]
[Route("api/v1/[controller]/[action]")] // localhost:port/api/v1/Auth
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    // localhost:port/api/v1/Auth/Home
    [HttpGet]
    public IActionResult Home()
    {
        return Ok(new { message = "Hello Auth" });
    }

    // localhost:port/api/v1/Auth/Register
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {

        var bcpUser = new User
        {
            Email = registerDto.Email,
            UserName = registerDto.Email,
            Fullname = registerDto.Fullname
        };

        var result = await _userManager.CreateAsync(bcpUser, registerDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        // send new user to rabbitmq
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://rabbitmq:1jj395qu@206.189.84.49:5672")
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare("akenarin-ex.auth.fanout", ExchangeType.Fanout, durable: true);
        channel.QueueDeclare("akenarin-q.auth.fanout", durable: true);
        channel.QueueBind("akenarin-q.auth.fanout", "akenarin-ex.auth.fanout", string.Empty);

        var uId = bcpUser.Id;
        var uFullname = bcpUser.Fullname;
        var message = new { uId, uFullname };
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

        channel.BasicPublish("akenarin-ex.auth.fanout", string.Empty, null, body);

        return Ok(new { message = "ลงทะเบียนสำเร็จ" });
    }

    // localhost:port/api/v1/Auth/Login
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "รหัสผ่านไม่ถูกต้อง" });
        }        

        return await CreateToken(user.Email!);
    }

    // create JWT token
    private async Task<IActionResult> CreateToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return NotFound();

        // เตรียม payload สำหรับสร้าง token
        var payload = new List<Claim>
            {
                new("userId", user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!)
            };

        // เพิ่ม role ของ user แต่ละคนเข้าไปใน payload ด้วย (role-base authentication)
        // var userRoles = await _userManager.GetRolesAsync(user);
        // if (userRoles != null)
        // {
        //     payload.AddRange(userRoles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));
        // }

        // ดึง JWT_KEY จาก appsettings.json
        var jwtKey = Encoding.UTF8.GetBytes(_configuration.GetSection("JWT_KEY").Value!);

        // เตรียมข้อมูล payload token / Algorithms / expire 
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(payload),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(jwtKey), SecurityAlgorithms.HmacSha256),
            Expires = DateTime.UtcNow.AddDays(7)
        };

        // สร้าง token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new
        {
            access_token = tokenHandler.WriteToken(token),
            expiration = token.ValidTo
        });

    }

    // get user's profile
    [Authorize]
    [HttpGet] // auth/profile
    public async Task<IActionResult> Profile() {
        var userId = User.Claims.First(p => p.Type == "userId").Value;
        var userProfile = await _userManager.FindByIdAsync(userId);
        if (userProfile == null) return NotFound();
        return Ok(new {
            userProfile.Id,
            userProfile.Fullname,
            userProfile.Email
        });
    }

}
