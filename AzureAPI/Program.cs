using System.Reflection;
using System.Text;
using AzureAPI;
using AzureAPI.Services;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    options.HttpsPort = 443; // Port HTTPS par défaut
});


builder.Services.AddSingleton(options =>
{
    var keyVaultUri = builder.Configuration["Azure_KeyVaultUri"];
    var clientId = builder.Configuration["Azure_ClientId"];
    var tenantId = builder.Configuration["Azure_TenantId"];

    var credentialOptions = new DefaultAzureCredentialOptions
    {
        TenantId = tenantId,
        ManagedIdentityClientId = clientId
    };

    var credential = new DefaultAzureCredential(credentialOptions);

    var clientOptions = new SecretClientOptions
    {
        Retry =
        {
            Delay = TimeSpan.FromSeconds(2),
            MaxDelay = TimeSpan.FromSeconds(10),
            MaxRetries = 3,
            Mode = Azure.Core.RetryMode.Exponential
        }
    };

    return new SecretClient(new Uri(keyVaultUri), credential, clientOptions);
});
// var keyVaultUri = builder.Configuration["Azure_KeyVaultUri"];
// var clientId = builder.Configuration["Azure_ClientId"];
// var tenantId = builder.Configuration["Azure_TenantId"];
//
// var credentialOptions = new DefaultAzureCredentialOptions
// {
//     TenantId = tenantId,
//     ManagedIdentityClientId = clientId
// };
//
// var credential = new DefaultAzureCredential(credentialOptions);
//
// if (!string.IsNullOrEmpty(keyVaultUri))
// {
//     builder.Configuration.AddAzureKeyVault(
//         new Uri(keyVaultUri),
//         credential);
// }

builder.Services.AddSingleton<SecretsManager>();
    
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var secretsManager = builder.Services.BuildServiceProvider().GetRequiredService<SecretsManager>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretsManager.JwtSecret)),
    };
});

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Version = "v1",
        Title = "Azure DESA Api"
    });
    
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddCors(c =>
{
    c.AddDefaultPolicy(p =>
    {
        p.AllowAnyOrigin();
        p.AllowAnyMethod();
        p.AllowAnyHeader();
    });
});

builder.Services.AddScoped<IJwtAuthService, JwtAuthService>();
builder.Services.AddScoped<IAzureService, AzureService>();
builder.Services.AddScoped<SqlConnection>(sp =>
{
    var secretsManager = sp.GetRequiredService<SecretsManager>();
    return new SqlConnection(secretsManager.DatabaseConnectionString);
});
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<BlobServiceClient>(sp =>
{
    var secretsManager = sp.GetRequiredService<SecretsManager>();
    return new BlobServiceClient(secretsManager.BlobStorageConnectionString);
});
builder.Services.AddScoped<EncryptionService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors();

app.MapControllers();


//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.Run();