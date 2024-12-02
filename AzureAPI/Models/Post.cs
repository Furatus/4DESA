using System.ComponentModel.DataAnnotations;

namespace AzureAPI.Models;

public class Post
{
    public Guid Id { get; set; }
    
    [Required]
    public string? Title { get; set; }
    
    [Required]
    public string? Content { get; set; }
    
    public string Author { get; set; }
    
    public DateTime Date { get; set; }
    
}