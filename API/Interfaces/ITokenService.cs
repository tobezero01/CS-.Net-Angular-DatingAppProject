using API.Entities;

namespace API;

public interface ITokenService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<string> CreateToken(AppUser user);
}
