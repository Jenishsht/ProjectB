using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("cannot get  token key");  //get the token key
        if (tokenKey.Length < 64)
            throw new Exception("Your token key needs to be >=64 character");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));  //symmetric key mean like for encription and decripition we used the same key ...secret string inot byte

        var claims = new List<Claim>    //user information
        { 
            new Claim(ClaimTypes.Email,user.UserEmail),
            new Claim(ClaimTypes.NameIdentifier,user.Id)
        };
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);            //creds =define  how the token will be sign an dtoken canot be modify
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),       //who the token belong to (user claim)
            Expires = DateTime.UtcNow.AddDays(7),       //how long token valid
            SigningCredentials = creds                  //proof of authenticity

        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);

    }
}
