using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService): BaseApiController
{

    [HttpPost("register")] //api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto){

        if(await UserExits(registerDto.UserName)) return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();
        var newUser = new AppUser{
            UserName = registerDto.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };
       
        context.Users.Add(newUser);
        await context.SaveChangesAsync();

        return new UserDto{
            Username = newUser.UserName,
            Token = tokenService.CreateToken(newUser)
        };        
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){
        var user = await context.Users.FirstOrDefaultAsync(x=>x.UserName == loginDto.Username.ToLower());

        if(user==null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if(computedHash[i]!=user.PasswordHash[i]) return Unauthorized("Invalid password");
        }
        return new UserDto{
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExits(string Username){
        return await context.Users.AnyAsync(x=>x.UserName.ToLower() == Username.ToLower());
    }
}
