using System.Security.Claims;
using AzureAPI.Models;
using AzureAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace AzureAPI.Controllers;

[Route("/api/profile/")]
[SwaggerResponse(400, "Mauvaise requête", null)]
[SwaggerResponse(405, "Méthode non autorisée", null)]
[SwaggerResponse(500, "Erreur Interne", null)]
[ApiController]

public class SocialMediaController : ControllerBase
{
    [Route("register")]
    [SwaggerResponse(200, "ok", typeof(Guid))]
    [SwaggerResponse(415, "Unsupported Media Type", null)]
    [HttpPost]
    public IActionResult Register(IAzureService azure, [FromBody] RegisterUserDto userDto)
    {
        try
        {
            if (userDto == null)
                 {
                     return BadRequest("Please provide user information");
                 }
            
            if (azure.GetUserByName(userDto.Username) != null) return BadRequest("Username already exists, please choose another one");
            
            var user = new User
            {
                Username = userDto.Username,
                Password = userDto.Password,
                Email = userDto.Email,
                IsPrivate = userDto.IsPrivate
            };
            var id = azure.RegsiterUser(user);

            return Created($"/api/stud/{id}", id);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [Route("getby/id")]
    [SwaggerResponse(200, "ok", typeof(User))]
    [HttpGet]
    public IActionResult GetUserById(IAzureService azure, [FromQuery] ItemId itemId)
    {
        try
        {
            var foundUser = azure.GetUserById(itemId.Id);
            if (foundUser == null)
            {
                return NotFound($"User id : {itemId.Id} not found");
            }

            return Ok(foundUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [Route("getby/username")]
    [SwaggerResponse(200, "ok", typeof(User))]
    [HttpGet]
    public IActionResult GetUserByName(IAzureService azure, [FromQuery] Username username)
    {
        try
        {
            var foundUser = azure.GetUserByName(username.Name);
            if (foundUser == null)
            {
                return NotFound($"User name : {username.Name} not found");
            }

            return Ok(foundUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [Route("update")]
    [HttpPatch]
    [SwaggerResponse(404, "Utilisateur non trouvé", null)]
    [SwaggerResponse(401, "Non Autorisé.", null)]
    [SwaggerResponse(200, "ok", null)]
    [Authorize]
    public IActionResult UpdateUser(IAzureService azure, [FromBody] UserUpdateDto userDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != userDto.Id.ToString()) return Unauthorized("You are not allowed to modify other users");
        azure.UpdateUser(userDto.Id, userDto);
        return Ok();
    }
    
    [HttpDelete]
    [Route("delete")]
    [SwaggerResponse(401, "Non Autorisé.", null)]
    [SwaggerResponse(404, "Utilisateur non trouvé", null)]
    [SwaggerResponse(200, "ok", null)]
    [Authorize]
    public IActionResult Delete(IAzureService azure, [FromQuery] ItemId itemId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != itemId.Id.ToString()) return Unauthorized("You are not allowed to delete other users");
        var id = itemId.Id;
        azure.DeleteUser(id);
        return Ok("Deleted user.");
    }
    
    [Route("togglePrivate")]
    [HttpPost]
    [SwaggerResponse(401, "Non Autorisé.", null)]
    [SwaggerResponse(200, "ok", null)]
    [Authorize]
    public IActionResult TogglePrivate(IAzureService azure, [FromQuery] ItemId itemId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != itemId.Id.ToString()) return Unauthorized("You are not allowed to delete other users");
        azure.TogglePrivate(itemId.Id);
        return Ok("Toggled private status.");
    }
}

[Route("/api/auth/")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost]
    [SwaggerResponse(401, "Non Autorisé.", null)]
    [SwaggerResponse(200, "ok", typeof(string))]
    [Route("login")]
    public IActionResult Login(IJwtAuthService jwtAuthService, [FromBody] Login login)
    {
        var user = jwtAuthService.Authenticate(login);
        if (user == null) return Unauthorized("User not found, wrong username/password");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var token = jwtAuthService.GenerateToken(
            Env.JwtSecret, claims);

        return Ok(token);
    }
}

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
            Date = DateTime.Now
        };
        
        var id = azure.CreatePost(post);
        return Created($"/api/post/{id}", id);
    }
        
    [HttpGet]
    [Route("getby/id")]
    [SwaggerResponse(200, "ok", typeof(Post))]
    [SwaggerResponse(403, "Interdit / profil privé", null)]
    public IActionResult GetPostById(IAzureService azure, [FromQuery] ItemId itemId)
    {
        var post = azure.GetPostById(itemId.Id);
        if (post == null) return NotFound("Post not found");
        return Ok(post);
    }
    
       
}