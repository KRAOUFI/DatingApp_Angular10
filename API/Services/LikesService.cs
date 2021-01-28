using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces.IServices;
using API.Repositories;

namespace API.Services
{
    public class LikesService : ILikesService
    {
        private readonly LikesRepository _likesRepo;
        private readonly UserRepository _userRepo;

        public LikesService(LikesRepository likesRepo, UserRepository userRepo)
        {
            _userRepo = userRepo;
            _likesRepo = likesRepo;
        }

        public async Task<Like> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _likesRepo.GetByConditionAsync(x => x.SourceId == sourceUserId && x.LikedId == likedUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _userRepo.AsQueryable().OrderBy(u => u.UserName).AsQueryable();
            var likes = _likesRepo.AsQueryable().AsQueryable();

            // Récupérer les likes du userId. 
            if (likesParams.Predicate == "liked")
            {
                // pour cela on récupère les les likes dont la sourceId == userId
                likes = likes.Where(like => like.SourceId == likesParams.UserId);

                // et de cette liste de like, sélectionner les user ayant fait l'objet de ces likes
                users = likes.Select(like => like.Liked);
            }

            // Récupérer les likes que userId a reçu
            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(u => u.LikedId == likesParams.UserId);
                users = likes.Select(x => x.Source);
            }

            var likedUsers = users.Select(x => new LikeDto
            {
                Username = x.UserName,
                KnownAs = x.KnownAs,
                Age = x.DateOfBirth.CalculateAge(),
                Id = x.Id,
                PhotoUrl = x.Photos.SingleOrDefault(x => x.IsMain).Url,
                City = x.City
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _userRepo.GetUserWithLikes(userId);
        }

        public async Task<string> AddLike(int sourceId, MemberDto likedUser)
        {
            var sourceUser = await this.GetUserWithLikes(sourceId);
            if (sourceUser == null) return "notfound";

            if (sourceUser.UserName == likedUser.Username) return "You can not like yourself";

            var userLike = await this.GetUserLike(sourceId, likedUser.Id);
            if (userLike != null) return "You already like this member";

            userLike = new Like
            {
                SourceId = sourceId,
                LikedId = likedUser.Id
            };
            
            _likesRepo.Add(userLike);
            if (await _likesRepo.SaveAsync() > 0) return "ok";

            return "Failed to like user";
        }
    }
}