using MagicLinkAuth.Application.Interfaces;
using MagicLinkAuth.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MagicLinkAuth.Api.Controllers;

[ApiController]
[Route("api/token")]
public class TestTokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public TestTokenController(ITokenService tokenService, IEmailService emailService)
    {
        _tokenService = tokenService;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult GetToken()
    {
        var user = new User { Email = "teste@exemplo.com" };
        var token = _tokenService.GenerateLoginToken(user, TimeSpan.FromMinutes(15));
        return Ok(new { token });
    }

    [HttpPost("request-login")]
    public async Task<IActionResult> RequestLogin([FromBody] LoginRequest request)
    {
        var user = new User { Email = request.Email };

        var token = _tokenService.GenerateLoginToken(user, TimeSpan.FromMinutes(15));

        var magicLink = $"https://localhost:3000/validate?token={token}";

        await _emailService.SendMagicLinkAsync(user.Email, magicLink);

        return Ok(new { Message = "Link enviado para seu e-mail." });
    }

    [HttpGet("auth/confirm")]
    public IActionResult Confirm([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token))
            return BadRequest("Token is required");

        var userId = _tokenService.ValidateMagicLinkTokenAsync(token);
        if (userId == null)
            return Unauthorized("Invalid or expired token");

        var jwt = _jwtService.GenerateToken(userId.Value);

        _tokenService.InvalidateTokenAsync(token);

        return Ok(new { token = jwt });
    }
}
