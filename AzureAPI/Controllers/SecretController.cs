using AzureAPI.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace AzureAPI.Controllers;

public class SecretController : ControllerBase
{
    private readonly SecretsManager _manager;
    
    public SecretController(SecretsManager manager)
    {
        _manager = manager;
    }
    
    [HttpGet]
    [Route("/api/secret/test")]
    public IActionResult GetSecret()
    {
        return Ok($"{_manager.JwtSecret}\n{_manager.DatabaseConnectionString}\n{_manager.PasswordHashKey}\n{_manager.BlobStorageConnectionString}");
    }
    
}