using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AzureAPI.Models;

public class ItemId
{
    [Key]
    [Required]
    public Guid Id { get; set; }
}