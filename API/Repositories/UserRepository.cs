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
    public class UserRepository : IRepository<User>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly DbSet<User> _entity;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _entity = _context.Set<User>();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _entity.FindAsync(id);
        }

        public async Task<User> GetByConditionAsync(Expression<Func<User, bool>> where) 
        {
            return await _entity.SingleOrDefaultAsync(where);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _entity.ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllByConditionAsync(Expression<Func<User, bool>> where)
        {
            return await _entity.Where(where).ToListAsync();
        }

        public async Task<bool> ExistAsync(Expression<Func<User, bool>> where)
        {
            return await _entity.AnyAsync(where);
        }

        public void Add(User entity)
        {
            _entity.Add(entity);
        }

        public void Update(User entity)
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

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IQueryable<User> AsQueryable() 
        {
            return _entity.AsQueryable();
        }

        #region Ces deux méthodes sont écrites de façon basiques pour retourner les données attendue. Le SQL généré n'est pas optimisé car inclus l'ensemble des champs des tables requêtées.
        /// <summary>
        /// Retourne l'entité User en incluant les photos associées, en fonction du username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<User> GetUserByUsernameAsync(string username) 
        {
            return await _entity
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username.ToLower());
        }

        /// <summary>
        /// Retourne l'ensemble des User en incluant les photos associées
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _entity
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<User> GetUserWithLikes(int userId) 
        {
            return await _entity
                .Include(u => u.Liked)
                .FirstOrDefaultAsync(x=>x.Id == userId);
        }
        #endregion        
    }
}
