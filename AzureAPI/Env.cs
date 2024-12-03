using System.Text;

namespace AzureAPI;

public static class Env
{
    public static string JwtSecret
    {
        get
        {
            /*DotEnv.Load();
            var envvars = DotEnv.Read();
            return envvars["JWT_SECRET"];*/
            //return Environment.GetEnvironmentVariable("JWT_SECRET")?? "DEFAULTSECRETKEY";
            return "";
        }
    }
    
    public static string dbString
    {
        get
        {
            //return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "";
            // Development connection string
            return "";
            //return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "Host=localhost;Database=ibay_api;Username=postgres;Password=postgres";
        }
    }
    
    public static string passwordHashKey
    {
        get
        {
            //return "b14ca5898a4e4133bbce2ea2315a1916";
            //return Environment.GetEnvironmentVariable("PASSWORD_HASH_KEY") ?? "DEFAULTPASSWORDHASHKEY";
            return "";
        }
    }
    
    public static string blobStoragePath
    {
        get
        {
            return "https://socialmediastorage.blob.core.windows.net/";
            //return Environment.GetEnvironmentVariable("BLOB_STORAGE_PATH") ?? "https://socialmediastorage.blob.core.windows.net/";
        }
    }
}