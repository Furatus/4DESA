using System.ComponentModel.DataAnnotations;

namespace AzureAPI.Models;

public class PostUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public string? Title { get; set; }
    
    public string? Content { get; set; }
}