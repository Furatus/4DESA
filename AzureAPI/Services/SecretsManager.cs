namespace AzureAPI.Services;

public class SecretsManager
{
    public string DatabaseConnectionString { get; private set; }
    public string JwtSecret { get; private set; }
    public string PasswordHashKey { get; private set; }

    public SecretsManager(IConfiguration configuration)
    {
        DatabaseConnectionString = configuration["DbConnectionString"];
        JwtSecret = configuration["JwtSecret"];
        PasswordHashKey = configuration["PasswordHashKey"];
    }
}