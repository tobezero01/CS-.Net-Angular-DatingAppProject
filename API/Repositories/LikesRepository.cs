using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;

namespace API.Repositories
{
    public class LikesRepository : ILikesRepository
    {
        public void AddLike(UserLike like)
        {
            throw new NotImplementedException();
        }

        public void DeleteLike(UserLike like)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
        {
            throw new NotImplementedException();
        }

        public Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MemberDto>> GetUserLikes(string predicate, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}