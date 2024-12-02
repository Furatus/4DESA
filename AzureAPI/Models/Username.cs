using System.ComponentModel.DataAnnotations;

namespace AzureAPI.Models;

public class Username
{
    [Key]
    [Required]
    public string Name { get; init; }
}