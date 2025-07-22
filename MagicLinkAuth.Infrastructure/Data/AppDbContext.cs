using MagicLinkAuth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MagicLinkAuth.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<LoginToken> LoginTokens => Set<LoginToken>();
    public DbSet<User> Users => Set<User>();
}
