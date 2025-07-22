using MagicLinkAuth.Domain.Entities;

namespace MagicLinkAuth.Application.Interfaces;

public interface ITokenService
{
    string GenerateLoginToken(User user, TimeSpan expiration);
}
