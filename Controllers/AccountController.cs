using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers;

[Authorize]
[ApiController]
public class AccountController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("v1/accounts/register")]
    public async Task<IActionResult> Post(
        [FromBody] RegisterViewModel model, 
        [FromServices] BlogDataContext context,
        [FromServices] EmailService emailService)
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

            emailService.Send(
                model.Name,
                model.Email,
                "Bem vindo",
                $"olá <strong>Josefino<strong> sua senha é {model.Password}",
                "Equipe TkN",
                "davi.frrs@outlook.com");

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
    public async Task<IActionResult> Login(
        [FromBody] LoginViewModel model,
        [FromServices] BlogDataContext context,
        [FromServices] TokenService tokenService)
    {
        if (!ModelState.IsValid) return BadRequest(new ResultViewModel<User>(ModelState.GetErrors()));

        var user = await context.Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == model.Email);

        if (user is null) 
            return StatusCode(401,new ResultViewModel<User>("Usuário ou senha inválido"));

        if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return StatusCode(401, new ResultViewModel<User>("Usuário ou senha inválido"));

        try
        {
            var token = tokenService.GenerateToken(user);
            
            return Ok(new ResultViewModel<string>(token));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("05X04 - Internal server error"));
        }
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