using AzureAPI.Models;
using AzureAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AzureAPI.Controllers;


[Route("/api/media/")]
[ApiController]
public class FileController : ControllerBase
{
    [HttpPost]
    [SwaggerResponse(401, "Non Autorisé.", null)]
    [SwaggerResponse(200, "ok", typeof(string))]
    [Route("upload")]
    
public IActionResult UploadFile(IBlobStorageService blobStorageService, [FromForm] IFormFile file, [FromForm] ItemId itemId)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No provided file.");
        }
        
        var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "video/mp4", "video/x-matroska" };
        if (!allowedMimeTypes.Contains(file.ContentType))
        {
            return BadRequest("Unsupported file type. Supported types are: jpeg, png, gif, mp4, mkv.");
        }

        var onlineFilePath  = blobStorageService.uploadFileToAzureBlob(file, itemId.Id);

        return Ok(new { Message = "Successfully uploaded file.", Path = onlineFilePath });
    }
}