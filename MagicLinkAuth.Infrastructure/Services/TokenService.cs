using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MagicLinkAuth.Application.Interfaces;
using MagicLinkAuth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;

    public TokenService(IConfiguration config, AppDbContext context)
    {
        _config = config;
        _context = context;
    }

    public async Task<string> GenerateAndStoreTokenAsync(User user, TimeSpan expiration)
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

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(securityToken);

        var loginToken = new LoginToken
        {
            Token = tokenString,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.Add(expiration)
        };

        _context.LoginTokens.Add(loginToken);
        await _context.SaveChangesAsync();

        return tokenString;
    }

    public async Task<Guid?> ValidateMagicLinkTokenAsync(string token)
    {
        var loginToken = await _context.LoginTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        if (loginToken == null || !loginToken.IsValid)
            return null;

        return loginToken.UserId;
    }

    public async Task InvalidateTokenAsync(string token)
    {
        var loginToken = await _context.LoginTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        if (loginToken != null && !loginToken.IsUsed)
        {
            loginToken.IsUsed = true;
            await _context.SaveChangesAsync();
        }
    }
}
