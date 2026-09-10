using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Entities;
using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using API.Extension;
using API.Services;

namespace API.Controllers;

public class AccountController(AppDbContext context,ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    //public async Task<ActionResult<AppUser>> Register(string email, string displayname,string password)
    public async Task<ActionResult<UserDTO>> Register(registerDTO registerdto)
    {
        if (await EmailExists(registerdto.Email)) return BadRequest("Email Taken");
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            DisplayName = registerdto.DisplayName,
            Email = registerdto.Email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.Password)),
            PasswordSalt = hmac.Key

        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        
       return user.ToDto(tokenService);


    }

    [HttpPost("login")]
     public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.Email == loginDto.Email);
        if (user == null) return Unauthorized("Invalid Eamil Address");
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for (var i = 0; i < ComputedHash.Length; i++)
        {
            if (ComputedHash[i] != user.PasswordHash[i]) return Unauthorized("Invlaid Password");

        }

        return user.ToDto(tokenService);


    }
    private async Task<bool> EmailExists(string email)
    {
        return await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
    }

}
