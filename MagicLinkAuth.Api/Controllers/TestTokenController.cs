using MagicLinkAuth.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost("sendEmail")]
    public async Task<IActionResult> RequestLogin([FromBody] LoginRequest request)
    {
        var token = await _tokenService.GenerateAndStoreTokenAsync(request.Email, TimeSpan.FromMinutes(15));
        var magicLink = $"http://localhost:5173/validate?token={token}";

        await _emailService.SendMagicLinkAsync(request.Email, magicLink);

        return Ok(new { Message = "Link enviado para seu e-mail." });
    }

    [HttpGet("validate")]
    public async Task<IActionResult> Validate([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Token é obrigatório.");

        // Valida o token e obtém o e-mail associado
        var email = await _tokenService.ValidateMagicLinkTokenAsync(token);
        if (email is null)
            return Unauthorized("Token inválido ou expirado.");

        // Gera o JWT com base no e-mail
        var jwt = _jwtService.GenerateToken(email);

        // Invalida o token magic link após o uso
        await _tokenService.InvalidateTokenAsync(token);

        return Ok(new { token = jwt });
    }

}
