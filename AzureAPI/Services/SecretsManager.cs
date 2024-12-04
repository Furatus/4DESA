using Azure.Security.KeyVault.Secrets;

namespace AzureAPI.Services;

public class SecretsManager
{
    public string DatabaseConnectionString { get; private set; }
    public string JwtSecret { get; private set; }
    public string PasswordHashKey { get; private set; }
    
    public string BlobStorageConnectionString { get; private set; }

    public SecretsManager(SecretClient secretClient)
    {
        DatabaseConnectionString = secretClient.GetSecret("DbConnectionString").Value.Value;
        JwtSecret = secretClient.GetSecret("JwtSecret").Value.Value;
        PasswordHashKey = secretClient.GetSecret("PasswordHashKey").Value.Value;
        BlobStorageConnectionString = secretClient.GetSecret("BlobStorageConnectionString").Value.Value;
    }
}