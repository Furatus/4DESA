﻿using System.ComponentModel.DataAnnotations;

namespace AzureAPI.Models;

public class Login
{ 
    [Required] 
    public string? Username { get; set; } 
    
    [Required] 
    public string? Password { get; set; }
}