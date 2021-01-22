using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.IRepositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class LikesRepository : IRepository<Like>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly DbSet<Like> _entity;

        public LikesRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _entity = _context.Set<Like>();
        }

        public async Task<Like> GetByIdAsync(int id)
        {
            return await _entity.FindAsync(id);
        }

        public async Task<Like> GetByConditionAsync(Expression<System.Func<Like, bool>> where)
        {
            return await _entity.SingleOrDefaultAsync(where);
        }

        public async Task<IEnumerable<Like>> GetAllAsync()
        {
            return await _entity.ToListAsync();
        }

        public async Task<IEnumerable<Like>> GetAllByConditionAsync(Expression<System.Func<Like, bool>> where)
        {
            return await _entity.Where(where).ToListAsync();
        }

        public async Task<bool> ExistAsync(Expression<System.Func<Like, bool>> where)
        {
            return await _entity.AnyAsync(where);
        }

        public async Task<int> CreateAsync(Like entity)
        {
            _entity.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Like entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var entityToDelete = await _entity.FindAsync(id);
            if (entityToDelete == null) 
            {
                return -1;
            }
            _entity.Remove(entityToDelete);
            return await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserWithLikes(int userId) 
        {
            return await _context.Users
                .Include(u => u.Liked)
                .FirstOrDefaultAsync(x=>x.Id == userId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likeParams) 
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();
            
            // Récupérer les likes du userId. 
            if (likeParams.Predicate == "liked") 
            {
                // pour cela on récupère les les likes dont la sourceId == userId
                likes = likes.Where(like => like.SourceId == likeParams.UserId);

                // et de cette liste de like, sélectionner les user ayant fait l'objet de ces likes
                users = likes.Select(like => like.Liked);
            }
            
            // Récupérer les likes que userId a reçu
            if (likeParams.Predicate == "likedBy") 
            {
                likes = likes.Where(u => u.LikedId == likeParams.UserId);
                users = likes.Select(x=>x.Source);
            }

            var likedUsers =  users.Select(x => new LikeDto 
            {
                Username = x.UserName,
                KnownAs = x.KnownAs,
                Age = x.DateOfBirth.CalculateAge(),
                Id = x.Id, 
                PhotoUrl = x.Photos.SingleOrDefault(x=>x.IsMain).Url,
                City = x.City
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likeParams.PageNumber, likeParams.PageSize);
        }
    }
}