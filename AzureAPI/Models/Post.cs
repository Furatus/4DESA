using System.ComponentModel.DataAnnotations;

namespace AzureAPI.Models;

public class Post
{
    public Guid Id { get; set; }
    
    [Required]
    public string? Title { get; set; }
    
    [Required]
    public string? Content { get; set; }
    
    public Guid Author { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ModifiedAt { get; set; }
    
    public string? MediaUrl { get; set; }
    
}