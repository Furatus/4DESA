

using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Text;
using AzureAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace AzureAPI.Services;

public class JwtAuthService : IJwtAuthService
{
    private IAzureService _azureService;
    
    public JwtAuthService(IAzureService azureService)
    {
        _azureService = azureService;
    }

    public User Authenticate(Login userlogin)
    {
        if (string.IsNullOrEmpty(userlogin.Username) || string.IsNullOrEmpty(userlogin.Password))
        {
            return null;
        }
        var loginginUser = _azureService.GetLogin(userlogin.Username);
        
        if (loginginUser == null)
        {
            return null;
        }

        var decryptedPassword = EncryptionService.DecryptString(loginginUser.Password);
        if (decryptedPassword != userlogin.Password)
        {
            return null;
        }
        return loginginUser;
    }

    public string GenerateToken(string secret, List<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = new SigningCredentials(
                key, 
                SecurityAlgorithms.HmacSha256)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}