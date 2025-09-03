using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{

    [Required]
    public string UserName { get; set; } = "";

    [Required]
    [EmailAddress]
    public string UserEmail { get; set; }= "";

    [Required]
    [MinLength(4)]
    public string Password { get; set; }= "";
    

    
}
