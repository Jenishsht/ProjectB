using System;
using API.Controllers;

namespace API.Data;

public class LoginDto 
{
    public string UserEmail { get; set; } = "";
    public string Password { get; set; } = "";
}
