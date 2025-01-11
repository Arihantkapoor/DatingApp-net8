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
        //getting secret key/token key from server
        var tokenKey = config["TokenKey"] ?? throw new Exception("TokenKey was not found in appsettings.json");

        if(tokenKey.Length<64) throw new Exception("Token Key needs to be longer");

        //creating key for encrypting/decrypting the token key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        //making claim for username
        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName)
        };

        //creating Credentials for the created key and algoritm used for creating token
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        //creating token description with details like subject, expiry date/time, credentials
        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };
        
        //making a JWT token handler class
        var tokenHandler = new JwtSecurityTokenHandler();

        //creating a token
        var token = tokenHandler.CreateToken(tokenDescriptor);

        //returing a token in string
        return tokenHandler.WriteToken(token);
    }
}   
