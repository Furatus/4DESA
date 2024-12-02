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
        if (userDto == null)
        {
            return BadRequest("User is null");
        }
        
        try
        {
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
            //return Ok(itemId.Id);
            //var userGuid = idService.ParseId(itemId.Id);
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
    
    [Route("test")]
    public IActionResult Test([FromQuery] string id)
    {
        var result = idService.ParseId(id);
        return Ok(result);
    }
}