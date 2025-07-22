namespace MagicLinkAuth.Application.Interfaces;

public interface IEmailService
{
    Task SendMagicLinkAsync(string toEmail, string magicLink);
}
