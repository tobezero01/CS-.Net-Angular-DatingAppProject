using API.DTOs;
using API.Helpers;

namespace API;

public interface ILikesRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <returns></returns>
    Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="likesParams"></param>
    /// <returns></returns>
    Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentUserId"></param>
    /// <returns></returns>
    Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="like"></param>
    void DeleteLike(UserLike like);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="like"></param>
    void AddLike(UserLike like);
}
