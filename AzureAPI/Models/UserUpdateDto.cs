using System.ComponentModel.DataAnnotations;
namespace AzureAPI.Models;

public class UserUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    
    public string? Username { get; set; }
    
    public string? Password { get; set; }
    
    public string? Email { get; set; }
    
}