using System.ComponentModel.DataAnnotations;

namespace AzureAPI.Models;

public class CreatePost
{
    [Required]
    public string? Title { get; set; }
    
    [Required]
    public string? Content { get; set; }
}