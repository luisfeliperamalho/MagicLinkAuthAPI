using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MagicLinkAuth.Application.Interfaces;
using MagicLinkAuth.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MagicLinkAuth.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;

    public TokenService(IConfiguration config, AppDbContext context)
    {
        _config = config;
        _context = context;
    }

    private readonly Dictionary<string, (Guid userId, DateTime expires)> _tokens = new();

    public string GenerateLoginToken(User user, TimeSpan expiration)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            }),
            Expires = DateTime.UtcNow.Add(expiration),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public void StoreToken(string token, Guid userId, TimeSpan validFor)
    {
        _tokens[token] = (userId, DateTime.UtcNow.Add(validFor));
    }

    public Guid? ValidateMagicLinkTokenAsync(string token)
    {
        if (_tokens.TryGetValue(token, out var entry))
        {
            if (entry.expires > DateTime.UtcNow)
                return entry.userId;
        }
        return null;
    }

    public void InvalidateTokenAsync(string token)
    {
        _tokens.Remove(token);
    }
}
