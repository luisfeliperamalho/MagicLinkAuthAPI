using System.Threading.Tasks;
using MagicLinkAuth.Application.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace MagicLinkAuth.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendMagicLinkAsync(string toEmail, string magicLink)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_config["Mail:SenderName"], _config["Mail:SenderEmail"]));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Seu link mágico de login";

        message.Body = new TextPart("plain")
        {
            Text = $"Clique neste link para entrar: {magicLink}"
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_config["Mail:Host"], int.Parse(_config["Mail:Port"]), false);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
