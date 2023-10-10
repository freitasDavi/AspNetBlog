using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[Authorize]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly TokenService _tokenService;
    
    public AccountController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    [AllowAnonymous]
    [HttpPost("v1/login")]
    public IActionResult Login()
    {
        var token = _tokenService.GenerateToken(null);

        return Ok(token);
    }

    
    [HttpGet("v1/user")]
    [Authorize(Roles = "user")]
    public IActionResult GetUser() => Ok(User.Identity.Name);
    
    
    [HttpGet("v1/author")]
    [Authorize(Roles = "author")]
    public IActionResult GetAuthor() => Ok(User.Identity.Name);
    
    
    [HttpGet("v1/admin")]
    [Authorize(Roles = "admin")]
    public IActionResult GetAdmin() => Ok(User.Identity.Name);
}