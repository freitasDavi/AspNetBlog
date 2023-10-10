using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blog.Models;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Services;

public class TokenService
{
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        // TokenHandler precisa que a string seja convertida para Byte[]
        var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
        // Configurações do token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
              new Claim(ClaimTypes.Name, "davifreitas"), // User.Identity.Name
              new Claim(ClaimTypes.Role, "admin"), // User.IsInRole
              new Claim(ClaimTypes.Role, "user"), // User.IsInRole
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        // Escreve uma string a partir do Token gerado
        return tokenHandler.WriteToken(token);
    }
}