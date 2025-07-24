using MagicLinkAuth.Domain.Entities;

namespace MagicLinkAuth.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAndStoreTokenAsync(String email, TimeSpan expiration);

    Task<string?> ValidateMagicLinkTokenAsync(string token);

    Task InvalidateTokenAsync(string token);
}
