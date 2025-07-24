using System.Threading.Tasks;
using MagicLinkAuth.Domain.Entities;

public interface IUserService
{
    Task<User> FindOrCreateByEmailAsync(string email);
}
