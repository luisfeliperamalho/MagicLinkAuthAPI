using MagicLinkAuth.Domain.Entities;

namespace MagicLinkAuth.Application.Interfaces;

public interface ITokenService
{
    string GenerateLoginToken(User user, TimeSpan expiration);

    void StoreToken(string token, Guid userId, TimeSpan validFor);

    Guid? ValidateMagicLinkTokenAsync(string token);
}
