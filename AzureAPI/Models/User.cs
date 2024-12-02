using System.ComponentModel.DataAnnotations;

namespace AzureAPI.Models;

public class User
{
    public Guid Id { get; set; }
    
    [Required]
    public string? Username { get; set; }
    
    [Required]
    public string? Password { get; set; }
    
    [Required]
    public string? Email { get; set; }

    public bool IsPrivate { get; set; } = false;
}
