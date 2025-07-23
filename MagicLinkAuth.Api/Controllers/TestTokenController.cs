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
    private readonly IJwtService _jwtService;

    public TestTokenController(ITokenService tokenService, IEmailService emailService, IJwtService jwtService)
    {
        _tokenService = tokenService;
        _emailService = emailService;
        _jwtService = jwtService;
    }

    [HttpPost("request-login")]
    public async Task<IActionResult> RequestLogin([FromBody] LoginRequest request)
    {
        var user = new User { Email = request.Email };

        // Gera e armazena o token magic link
        var token = await _tokenService.GenerateAndStoreTokenAsync(user, TimeSpan.FromMinutes(15));

        var magicLink = $"http://localhost:5209/api/token/auth/confirm?token={token}";

        await _emailService.SendMagicLinkAsync(user.Email, magicLink);

        return Ok(new { Message = "Link enviado para seu e-mail." });
    }

    [HttpGet("auth/confirm")]
    public async Task<IActionResult> Confirm([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token))
            return BadRequest("Token é obrigatório.");

        var userId = await _tokenService.ValidateMagicLinkTokenAsync(token);
        if (userId == null)
            return Unauthorized("Token inválido ou expirado.");

        var jwt = _jwtService.GenerateToken(userId.Value);

        await _tokenService.InvalidateTokenAsync(token);

        return Ok(new { token = jwt });
    }
}
