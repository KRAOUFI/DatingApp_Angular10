using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces.IServices;
using API.Repositories;

namespace API.Services
{
    public class LikesService : ILikesService
    {
        private readonly LikesRepository _likesRepo;

        public LikesService(LikesRepository likesRepo, UserRepository userRepo)
        {
            _likesRepo = likesRepo;
        }

        public async Task<Like> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _likesRepo.GetByConditionAsync(x => x.SourceId == sourceUserId && x.LikedId == likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            return await _likesRepo.GetUserLikes(likesParams);
        }

        public async Task<User> GetUserWithLikes(int userId)
        {
            return await _likesRepo.GetUserWithLikes(userId);
        }

        public async Task<string> AddLike(int sourceId, MemberDto likedUser) 
        {
            var sourceUser = await this.GetUserWithLikes(sourceId);
            if (sourceUser == null) return "notfound";
            
            if(sourceUser.UserName == likedUser.Username) return "You can not like yourself";
            
            var userLike = await this.GetUserLike(sourceId, likedUser.Id);
            if(userLike!=null) return "You already like this member";

            userLike = new Like
            { 
                SourceId = sourceId,
                LikedId = likedUser.Id
            };
            
            if (await _likesRepo.CreateAsync(userLike) > 0) return "ok";

            return "Failed to like user";

        }
    }
}