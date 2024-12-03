using System.Security.Claims;
using AzureAPI;
using AzureAPI.Models;
using AzureAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AzureAPI.Controllers;

[Route("/api/auth/")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost]
    [SwaggerResponse(401, "Non Autorisé.", null)]
    [SwaggerResponse(200, "ok", typeof(string))]
    [Route("login")]
    public IActionResult Login(IJwtAuthService jwtAuthService, [FromBody] Login login)
    {
        var user = jwtAuthService.Authenticate(login);
        if (user == null) return Unauthorized("User not found, wrong username/password");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var token = jwtAuthService.GenerateToken(
            Env.JwtSecret, claims);

        return Ok(token);
    }
}