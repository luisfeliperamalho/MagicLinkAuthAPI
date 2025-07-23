using MagicLinkAuth.Domain.Entities;

namespace MagicLinkAuth.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateAndStoreTokenAsync(User user, TimeSpan expiration);

    Task<Guid?> ValidateMagicLinkTokenAsync(string token);

    Task InvalidateTokenAsync(string token);
}
