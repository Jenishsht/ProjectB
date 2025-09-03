using System;
using System.Formats.Asn1;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(AppDbContext context ,ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")] //api/account/register


    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        if (await EmailExits(registerDto.UserEmail)) return BadRequest("Email taken");


        using var hmac = new HMACSHA512();  // used to hash the password

        var user = new AppUser
        {
            UserName = registerDto.UserName,
            UserEmail = registerDto.UserEmail,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.ToDto(tokenService);
       
    }


    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.UserEmail == loginDto.UserEmail);
        if (user == null) return Unauthorized("Invalid email Address");

        using var hmac = new HMACSHA512(user.PasswordSalt);  //check with the  paswword that we creat in the register
        var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for (var i = 0; i < ComputeHash.Length; i++)
        {
            if (ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }
        return user.ToDto(tokenService);
       
    } 

    private async Task<bool> EmailExits(String email)
    {
        return await context.Users.AnyAsync(x => x.UserEmail.ToLower() == email.ToLower());
    }


}
