using CleanArchitecture.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers;

/// <summary>
/// Demo authentication endpoint.
///
/// This is a stub for the sample: it checks the supplied credentials against
/// the "DemoUser" section of configuration and issues a JWT. A production
/// system would validate users against a real identity provider instead.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthController(IJwtTokenService tokenService, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public record LoginRequest(string Username, string Password);

    public record LoginResponse(string Token);

    /// <summary>Exchanges demo credentials for a JWT access token.</summary>
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        var expectedUser = _configuration["DemoUser:Username"];
        var expectedPassword = _configuration["DemoUser:Password"];

        if (request.Username != expectedUser || request.Password != expectedPassword)
            return Unauthorized(new { title = "Unauthorized", detail = "Invalid credentials." });

        var token = _tokenService.CreateToken(request.Username);
        return Ok(new LoginResponse(token));
    }
}
