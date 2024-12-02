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

            return Environment.GetEnvironmentVariable("JWT_SECRET")?? "DEFAULTSECRETKEY";
        }
    }
    
    public static string dbString
    {
        get
        {
            //return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "";
            // Development connection string
            return
                "Server=tcp:4desadatabaseservice.database.windows.net,1433;Initial Catalog=SocialMediaDB;Persist Security Info=False;User ID=SocialMediaAdmin;Password=9]VGT+nsYB)>7-6;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "Host=localhost;Database=ibay_api;Username=postgres;Password=postgres";
        }
    }
}