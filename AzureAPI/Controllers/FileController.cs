using System.Security.Claims;
using AzureAPI.Models;
using AzureAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AzureAPI.Controllers;


[Route("/api/media/")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IAzureService _azureService;
    private readonly IBlobStorageService _blobStorageService;

    public FileController(IAzureService azureService, IBlobStorageService blobStorageService)
    {
        _azureService = azureService;
        _blobStorageService = blobStorageService;
    }
    
    [HttpPost]
    [SwaggerResponse(401, "Non Autorisé.", null)]
    [SwaggerResponse(404, "Post non trouvé", null)]
    [SwaggerResponse(500, "Erreur Interne", null)]
    [SwaggerResponse(200, "ok", typeof(string))]
    [Route("upload")]
    [Authorize]
    
public IActionResult UploadFile([FromForm] FileUpload file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var post = _azureService.GetPostById(file.PostId);
        if (post == null)
        {
            return NotFound("Post not found");
        }
        
        if (post.Author.ToString() != userId)
        {
            return Unauthorized("You are not the author of this post.");
        }
        
        if (file.File == null || file.File.Length == 0)
        {
            return BadRequest("No provided file.");
        }
        
        var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "video/mp4", "video/x-matroska" };
        if (!allowedMimeTypes.Contains(file.File.ContentType))
        {
            return BadRequest("Unsupported file type. Supported types are: jpeg, png, gif, mp4, mkv.");
        }
        
        if (!string.IsNullOrEmpty(post.MediaUrl))
        {
            _blobStorageService.deleteFileFromAzureBlob(post.MediaUrl);
        }

        var onlineFilePath  = _blobStorageService.uploadFileToAzureBlob(file.File, file.PostId);

        return Ok(new { Message = "Successfully uploaded file.", Path = onlineFilePath });
    }

[HttpDelete]
[SwaggerResponse(200, "ok", typeof(string))]
[SwaggerResponse(401, "Non Autorisé.", null)]
[SwaggerResponse(404, "Post non trouvé", null)]
[SwaggerResponse(500, "Erreur Interne", null)]
[Route("delete")]
[Authorize]

public IActionResult DeleteFileIfExisting([FromQuery] ItemId itemId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var post = _azureService.GetPostById(itemId.Id);
        if (post == null)
        {
            return NotFound("Post not found");
        }
        
        if (post.Author.ToString() != userId)
        {
            return Unauthorized("You are not the author of this post.");
        }

        var uri = post.MediaUrl;
        
        _blobStorageService.deleteFileFromAzureBlob(uri);
        _azureService.UploadMedia(post.Id, null);
        
        return Ok("File deleted successfully.");
    }
}