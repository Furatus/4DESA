using System.Security.Claims;
using AzureAPI.Models;

namespace AzureAPI.Services;
public interface IJwtAuthService
{
    public User Authenticate(Login userlogin);

    public string GenerateToken(string secret, List<Claim> claims);
}