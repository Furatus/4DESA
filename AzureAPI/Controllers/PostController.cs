using System.Security.Claims;
using AzureAPI.Models;
using AzureAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Route("/api/post/")]
[ApiController]
[SwaggerResponse(400, "Mauvaise requête", null)]
[SwaggerResponse(405, "Méthode non autorisée", null)]
[SwaggerResponse(500, "Erreur Interne", null)]
public class PostController : ControllerBase
{
    [HttpPost]
    [Route("create")]
    [SwaggerResponse(401, "Non Autorisé.", null)]
    [SwaggerResponse(200, "ok", typeof(Guid))]
    [Authorize]
    public IActionResult CreatePost(IAzureService azure, [FromBody] CreatePost cpost)
    {
        if (cpost == null || cpost.Title == null)
        {
            return BadRequest("Please provide post information (at least a title)");
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = cpost.Title,
            Content = cpost.Content,
            Author = Guid.Parse(userId),
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now
        };
        
        var id = azure.CreatePost(post);
        return Created($"/api/post/{id}", id);
    }
        
    [HttpGet]
    [Route("getby/id")]
    [SwaggerResponse(200, "ok", typeof(Post))]
    [SwaggerResponse(400, "Interdit / profil privé", null)]
    public IActionResult GetPostById(IAzureService azure, [FromQuery] ItemId itemId)
    {
        var requestingUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        var post = azure.GetPostById(itemId.Id);
        if (post == null) return NotFound("Post not found");

        var author = azure.GetUserById(post.Author);
        if (author.IsPrivate && requestingUser != post.Author.ToString()) return BadRequest("This profile is private");
        return Ok(post);
    }
    
    [HttpGet]
    [Route("getfrom/Username")]
    [SwaggerResponse(200, "ok", typeof(List<Post>))]
    [SwaggerResponse(400, "Interdit / profil privé", null)]
    public IActionResult GetPostsFromUsername(IAzureService azure, [FromQuery] Username username)
    {
        var user = azure.GetUserByName(username.Name);
        if (user == null) return NotFound("User not found");
        if (user.IsPrivate)
        {
            var requestingUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            if (requestingUser != user.Id.ToString()) return BadRequest("This profile is private");
        }
        var posts = azure.GetPostsfromUserId(user.Id);
        return Ok(posts);
    }
       
    [HttpGet]
    [Route("getfrom/UserId")]
    [SwaggerResponse(200, "ok", typeof(List<Post>))]
    [SwaggerResponse(400, "Interdit / profil privé", null)]
    public IActionResult GetPostsFromUserId(IAzureService azure, [FromQuery] ItemId itemId)
    {
        var user = azure.GetUserById(itemId.Id);
        if (user == null) return NotFound("User not found");
        if (user.IsPrivate)
        {
            var requestingUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            if (requestingUser != user.Id.ToString()) return BadRequest("This profile is private");
        }
        var posts = azure.GetPostsfromUserId(itemId.Id);
        return Ok(posts);
    }
    
    [HttpPatch]
    [Route("update")]
    [SwaggerResponse(401, "Non Autorisé.", null)]
    [SwaggerResponse(200, "ok", null)]
    [Authorize]
    public IActionResult UpdatePost(IAzureService azure, [FromBody] PostUpdateDto postUpdateDto)
    {
        if (postUpdateDto == null) return BadRequest("Please provide post information");
        var dbpost = azure.GetPostById(postUpdateDto.Id);
        if (dbpost == null) return NotFound("Post not Found");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != dbpost.Author.ToString()) return Unauthorized("You are not allowed to modify other users posts");
        azure.UpdatePost(postUpdateDto.Id, postUpdateDto);
        return Ok($"Post {postUpdateDto.Id} edited.");
    }
    
    [HttpDelete]
    [Route("delete")]
    [SwaggerResponse(401, "Non Autorisé.", null)]
    [SwaggerResponse(200, "ok", typeof(string))]
    [SwaggerResponse(500, "Erreur Interne", null)]
    [Authorize]
    public IActionResult DeletePost(IBlobStorageService blobStorage, IAzureService azure, [FromQuery] ItemId itemId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var post = azure.GetPostById(itemId.Id);
        if (post == null) return NotFound("Post not found");
        if (post.Author.ToString() != userId) return Unauthorized("You are not the author of this post.");
        if (post.MediaUrl != null) blobStorage.deleteFileFromAzureBlob(post.MediaUrl);
        azure.DeletePost(itemId.Id);
        return Ok($"Post {itemId.Id} deleted.");
    }
    
}