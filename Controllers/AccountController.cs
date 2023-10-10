using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

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
    [HttpPost("v1/accounts/register")]
    public async Task<IActionResult> Post([FromBody] RegisterViewModel model, [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid) return BadRequest(new ResultViewModel<User>(ModelState.GetErrors()));
        
        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email.Replace("@", "-").Replace(".", "-")
        };

        user.PasswordHash = PasswordHasher.Hash(model.Password);

        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return new ObjectResult(new ResultViewModel<dynamic>(new
            {
                user = user.Email,
                password = model.Password
            }))
            {
                StatusCode = 201
            };

        }
        catch (DbUpdateException ex)
        {
            return StatusCode(400, new ResultViewModel<dynamic>("05X99 - Este email já está cadastrado"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<dynamic>("05X98 - Falha interna do servidor"));
        }
    }
    
    [AllowAnonymous]
    [HttpPost("v1/accounts/login")]
    public IActionResult Login()
    {
        var token = _tokenService.GenerateToken(null);

        return Ok(token);
    }

    #region AuthTest
    
    [HttpGet("v1/user")]
    [Authorize(Roles = "user")]
    public IActionResult GetUser() => Ok(User.Identity.Name);
    
    
    [HttpGet("v1/author")]
    [Authorize(Roles = "author")]
    public IActionResult GetAuthor() => Ok(User.Identity.Name);
    
    
    [HttpGet("v1/admin")]
    [Authorize(Roles = "admin")]
    public IActionResult GetAdmin() => Ok(User.Identity.Name);
    
    #endregion
}