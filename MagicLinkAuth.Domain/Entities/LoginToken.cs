public class LoginToken
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
}
