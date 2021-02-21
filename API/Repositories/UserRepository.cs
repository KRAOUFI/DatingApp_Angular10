using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.IRepositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class UserRepository : IRepository<AppUser>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly DbSet<AppUser> _entity;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _entity = _context.Set<AppUser>();
        }

        public async Task<AppUser> GetByIdAsync(int id)
        {
            return await _entity.FindAsync(id);
        }

        public async Task<AppUser> GetByConditionAsync(Expression<Func<AppUser, bool>> where) 
        {
            return await _entity.SingleOrDefaultAsync(where);
        }

        public async Task<IEnumerable<AppUser>> GetAllAsync()
        {
            return await _entity.ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetAllByConditionAsync(Expression<Func<AppUser, bool>> where)
        {
            return await _entity.Where(where).ToListAsync();
        }

        public async Task<bool> ExistAsync(Expression<Func<AppUser, bool>> where)
        {
            return await _entity.AnyAsync(where);
        }

        public void Add(AppUser entity)
        {
            _entity.Add(entity);
        }

        public void Update(AppUser entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            var theEntity = await _entity.FindAsync(id);
            if (theEntity != null)
            {
                _entity.Remove(theEntity);
            }
        }

        public IQueryable<AppUser> AsQueryable() 
        {
            return _entity.AsQueryable();
        }

        #region Ces deux méthodes sont écrites de façon basiques pour retourner les données attendue. Le SQL généré n'est pas optimisé car inclus l'ensemble des champs des tables requêtées.
        /// <summary>
        /// Retourne l'entité User en incluant les photos associées, en fonction du username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<AppUser> GetUserByUsernameAsync(string username) 
        {
            return await _entity
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username.ToLower());
        }

        /// <summary>
        /// Retourne l'ensemble des User en incluant les photos associées
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _entity
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<AppUser> GetUserWithLikes(int userId) 
        {
            return await _entity
                .Include(u => u.Liked)
                .FirstOrDefaultAsync(x=>x.Id == userId);
        }

        public async Task<string> GetUserGender(string username) 
        {
            return await _entity
                .Where(x=>x.UserName == username)
                .Select(x=>x.Gender)
                .FirstOrDefaultAsync();
        }
        #endregion        
    }
}
