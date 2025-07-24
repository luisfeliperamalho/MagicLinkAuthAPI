using MagicLinkAuth.Application.Interfaces;
using MagicLinkAuth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;

    public TokenService(IConfiguration config, AppDbContext context)
    {
        _config = config;
        _context = context;
    }

    // Gera e armazena um token associado apenas ao email (sem vincular a um usuário do sistema)
    public async Task<string> GenerateAndStoreTokenAsync(string email, TimeSpan expiration)
    {
        var token = Guid.NewGuid().ToString();

        var loginToken = new LoginToken
        {
            Token = token,
            Email = email,
            ExpiresAt = DateTime.UtcNow.Add(expiration),
            IsUsed = false
        };

        _context.LoginTokens.Add(loginToken);
        await _context.SaveChangesAsync();

        return token;
    }

    // Valida o token magic link e retorna o email, se válido e não expirado
    public async Task<string?> ValidateMagicLinkTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var loginToken = await _context.LoginTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        if (loginToken is null || loginToken.IsUsed || loginToken.ExpiresAt < DateTime.UtcNow)
            return null;

        return loginToken.Email;
    }

    // Invalida o token após o uso
    public async Task InvalidateTokenAsync(string token)
    {
        var loginToken = await _context.LoginTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        if (loginToken is not null && !loginToken.IsUsed)
        {
            loginToken.IsUsed = true;
            await _context.SaveChangesAsync();
        }
    }
}
