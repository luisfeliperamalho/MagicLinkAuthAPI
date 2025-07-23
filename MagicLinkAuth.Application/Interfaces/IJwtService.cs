public interface IJwtService
{
    string GenerateToken(Guid userId);
}