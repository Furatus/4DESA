using System.ComponentModel.DataAnnotations;

namespace AzureAPI.Models;

public class FileUpload
{
    [Required]
    public Guid PostId { get; set; }
    
    [Required]
    public IFormFile? File { get; set; }
}