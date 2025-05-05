using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API;

public interface IUserRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    void Update(AppUser user);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<AppUser>> GetUsersAsync();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<AppUser?> GetUserByIdAsync(int id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<AppUser?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userParams"></param>
    /// <returns></returns>
    Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="isCurrentUser"></param>
    /// <returns></returns>
    Task<MemberDto?> GetMemberAsync(string username, bool isCurrentUser);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="photoId"></param>
    /// <returns></returns>
    Task<AppUser?> GetUserByPhotoId(int photoId); 
}
