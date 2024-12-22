namespace API.Interfaces;
using Entities;

public interface ITokenService
{
    string CreateToken(AppUser user);
}
