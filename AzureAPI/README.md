# Azure Linkup API - a student project about an asp.net simple api, hosted on Azure.

## How to run ?

This project doens't need to be run, as it is already running on Azure Cloud.

However, if you want to run it on your own, you can launch it like any other .net Project, run `dotnet restore`, `dotnet build`  and `dotnet run`.

Link to Azure web app if still open : [Azure Linkup API](https://desalinkup-api-hvfrdvb5cngrb5hq.francecentral-01.azurewebsites.net/)

Link to Swagger documentation if still open : [Swagger Documentation](https://desalinkup-api-hvfrdvb5cngrb5hq.francecentral-01.azurewebsites.net/swagger/index.html)

## Environment Variables fetched on Azure Key Vault
- `JwtSecret` : secret of all the jwt tokens that will run on your api. (identity self-managed)
- `DbConnectionString` : connection string of the database. (hosted on azure SQL)
- `BlobStorageConnectionString` : connection string of the azure container storage. (hosted on azure blob storage)
- `PasswordHash` : 32 bytes password hash for the user database. (identity self-managed)

## Environment variables to add in appsettings.json or in environment variables on your azure web app
- `Azure_KeyVaultUri` : the uri of your azure key vault, to fetch the environment variables.
- `Azure_ClientId` : the client id of your azure app registration, to fetch the environment variables.
- `Azure_TenantId` : the tenant id of your azure org, to fetch the environment variables.

## Azure Services used
- Azure SQL : to host the database of the api.
- Azure Blob Storage : to host the images of the posts.
- Azure Key Vault : to store the environment variables of the api.
- Azure App Service : to host the api itself.
- Azure App Service Plan : to deploy the app service.
- Azure Managed Identity : to fetch the environment variables from the key vault with a secure user.
- and other miscellaneous services...

## Database Structure

The database is composed of 2 tables :
- Users : 
    - Id : Guid
    - Username : string
    - Email : string
    - Password : string
    - Isprivate : Boolean

- Posts :
    - Id : Guid
    - Title : string
    - Content : string
    - Author : FK(Guid -> Users(id))
    - MediaUrl : string
    - CreatedAt : DateTime
    - ModifiedAt : DateTime

## Filling Database

You can use the api itself (using swagger or your own http client) to fill the database, however, please mind that your user must be logged to create posts and to interract with authorized endpoints.

## Documentation

a openapi/swagger doc is provided to specify what are the endpoints of the api or the responses you can wait of it. you can access it by {your_address}/swagger

## Issues ?

this is a student project, it is not meant to be perfect or being flawless, but we probably won't resolve any issues you will open if you do so , so please no need to open issues.

However, if you have any questions about how this program works or about the code itself, don't wonder to message me and ask your question. thanks :)