using AzureAPI.Models;

namespace AzureAPI.Services;

public interface IAzureService
{
    public Guid RegsiterUser(User user);
    public User GetUserById(Guid id);
    public User GetUserByName(string username);
    
    public User? GetLogin(string username);
    public void UpdateUser(Guid id, UserUpdateDto user);
    public void DeleteUser(Guid id);
    public void setPrivate();
    public Guid CreatePost(Post post);
    public Post GetPostById(Guid id);
    public List<Post> GetPostsfromUserId(Guid id);
    public List<Post> GetPostsfromUsername(string username);
    public void UpdatePost(Guid id, Post post);
    public void UploadMedia(Guid id, string media);
    public void DeletePost(Guid id);
}