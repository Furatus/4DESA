using AzureAPI.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AzureAPI.Services;

public class AzureService : IAzureService
{
    private readonly SqlConnection _dbConnection;
    
    public AzureService(SqlConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public Guid RegsiterUser(User user)
    {
        try
        {
            _dbConnection.Open();
            user.Id = Guid.NewGuid();
            user.IsPrivate = false;

            var query =
                "INSERT INTO Users (Id, Username, Password, Email, IsPrivate) VALUES (@Id, @Username, @Password, @Email, @IsPrivate)";
            var parameters = new
            {
                Id = user.Id, Username = user.Username, Password = user.Password, Email = user.Email,
                IsPrivate = user.IsPrivate
            };
            
            _dbConnection.Execute(query, parameters);

            return user.Id;
        }
        finally
        {
            _dbConnection.CloseAsync();
        }
    }
    
    public User? GetUserById(Guid id)
    {
        try
        {
            _dbConnection.Open();
            var query = "SELECT Id, Username, Password, Email, IsPrivate FROM Users WHERE Id = @Id";
            var parameters = new {Id = id};
            var dbuser = _dbConnection.QuerySingleOrDefault<User>(query, parameters);
            
            if (dbuser == null)
            {
                return null;
            }
            
            var user = new User
            {
                Id = dbuser.Id,
                Username = dbuser.Username,
                Password = dbuser.Password,
                Email = dbuser.Email,
                IsPrivate = dbuser.IsPrivate
            };

            return user;
        }
        finally
        {
            _dbConnection.CloseAsync();
        }
    }
    
    public User? GetUserByName(string username)
    {
        throw new NotImplementedException();
    }
    
    public void UpdateUser(Guid id, User user)
    {
        throw new NotImplementedException();
    }
    
    public void DeleteUser(Guid id)
    {
        throw new NotImplementedException();
    }
    
    public void setPrivate()
    {
        throw new NotImplementedException();
    }
    
    public Guid CreatePost(Post post)
    {
        throw new NotImplementedException();
    }
    
    public Post GetPostById(Guid id)
    {
        throw new NotImplementedException();
    }
    
    public List<Post> GetPostsfromUserId(Guid id)
    {
        throw new NotImplementedException();
    }
    
    public List<Post> GetPostsfromUsername(string username)
    {
        throw new NotImplementedException();
    }
    
    public void UpdatePost(Guid id, Post post)
    {
        throw new NotImplementedException();
    }
    
    public void DeletePost(Guid id)
    {
        throw new NotImplementedException();
    }
}