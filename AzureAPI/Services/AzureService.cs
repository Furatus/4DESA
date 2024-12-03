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
            user.Password = EncryptionService.EncryptString(user.Password);

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
            var query = "SELECT Id, Username, Email, IsPrivate FROM Users WHERE Id = @Id";
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
        try
        {
            _dbConnection.Open();
            var query = "SELECT Id, Username, Email, IsPrivate FROM Users WHERE CAST(Username as nvarchar) = @Username";
            var parameters = new {Username = username};
            var dbuser = _dbConnection.QuerySingleOrDefault<User>(query, parameters);
            
            if (dbuser == null)
            {
                return null;
            }
            
            var user = new User
            {
                Id = dbuser.Id,
                Username = dbuser.Username,
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
    
    public User? GetLogin(string username)
    {
        try
        {
            _dbConnection.Open();
            var query = "SELECT Id, Username, Password, Email, IsPrivate FROM Users WHERE CAST(Username as nvarchar) = @Username";
            var parameters = new {Username = username};
            var dbuser = _dbConnection.QuerySingleOrDefault<User>(query, parameters);
            
            if (dbuser == null)
            {
                return null;
            }
            
            var login = new User
            {
                Id = dbuser.Id,
                Username = dbuser.Username,
                Password = dbuser.Password,
                Email = dbuser.Email,
                IsPrivate = dbuser.IsPrivate
            };

            return login;
        }
        finally
        {
            _dbConnection.CloseAsync();
        }
    }
    
    public void UpdateUser(Guid id, UserUpdateDto user)
    {
        try
        {
            if (user.Username == null && user.Password == null && user.Email == null)
            {
                throw new Exception("No data to update");
            }
            var checkifUsernameAlreadyExists = GetUserByName(user.Username);
            if (checkifUsernameAlreadyExists != null) throw new Exception("Username already exists");

            var queryBuildString = "";
            if (user.Username != null) queryBuildString += "Username = @Username";
            if (user.Password != null) queryBuildString += ",Password = @Password";
            if (user.Email != null) queryBuildString += ",Email = @Email";
            if (queryBuildString.StartsWith(',')) queryBuildString = queryBuildString.Remove(0, 1);
            
            _dbConnection.Open();
            var query = $"UPDATE Users SET {queryBuildString} WHERE Id = @Id";
            var parameters = new
            {
                Id = id, Username = user.Username, Password = EncryptionService.EncryptString(user.Password), Email = user.Email
            };
            
            _dbConnection.Execute(query, parameters);
        }
        finally
        {
            _dbConnection.CloseAsync();
        }
    }
    
    public void DeleteUser(Guid id)
    {
        try
        {
            _dbConnection.Open();
            var query = "DELETE FROM Users WHERE Id = @Id";
            var parameters = new {Id = id};
            
            _dbConnection.Execute(query, parameters);
        }
        finally
        {
            _dbConnection.CloseAsync();
        }
    }
    
    public void TogglePrivate(Guid id)
    {
        try
        {
            _dbConnection.Open();
            var query = "UPDATE Users SET IsPrivate = ~IsPrivate WHERE Id = @Id";
            var parameters = new {Id = id};
            
            _dbConnection.Execute(query, parameters);
        }
        finally
        {
            _dbConnection.CloseAsync();
        
        }
    }
    
    public Guid CreatePost(Post post)
    {
        try
        {
            _dbConnection.Open();
            var query =
                "INSERT INTO Posts (id, Title, Content, Author, media_Url, CreatedAt) VALUES (@Id, @Title, @Content, @Author, @MediaUrl, @CreatedAt)";
            var parameters = new
            {
                Id = post.Id, Title = post.Title, Content = post.Content, Author = post.Author, MediaUrl = "", CreatedAt = DateTime.Now
            };
            
            _dbConnection.Execute(query, parameters);
            
            return post.Id;

        }
        finally
        {
            _dbConnection.CloseAsync();
        }
    }
    
    public Post MapPost(dynamic dbpost)
    {
        return new Post
        {
            Id = dbpost.Id,
            Title = dbpost.Title,
            Content = dbpost.Content,
            Author = dbpost.Author,
            Date = DateTime.SpecifyKind(dbpost.CreatedAt, DateTimeKind.Utc),
            MediaUrl = dbpost.Media_Url
        };
    }
    
    public Post GetPostById(Guid id)
    {
        try
        {
            _dbConnection.Open();
            var query = "SELECT Id, Title, Content, Author, CreatedAt, Media_Url FROM Posts WHERE Id = @Id";
            var parameters = new {Id = id};
            var dbpost = _dbConnection.Query(query, parameters).Select(MapPost).SingleOrDefault();
            
            if (dbpost == null)
            {
                return null;
            }

            return dbpost;
        }
        finally
        {
            _dbConnection.CloseAsync();
        }
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
    
    public void UploadMedia(Guid id, string media)
    {
        throw new NotImplementedException();
    }
    
    public void DeletePost(Guid id)
    {
        throw new NotImplementedException();
    }
}