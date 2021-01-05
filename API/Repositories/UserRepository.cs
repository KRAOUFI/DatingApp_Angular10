using API.Data;
using API.DTOs;
using API.Entities;
using API.IRepositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

        public async Task<int> CreateAsync(User entity)
        {
            _entity.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(User entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var theEntity = await _entity.FindAsync(id);
            if (theEntity == null)
            {
                return -1;
            }

            _entity.Remove(theEntity);
            return await _context.SaveChangesAsync();;
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
        #endregion

        #region En utilisant ProjectTo, nous optimisons les requêtes générées
        public async Task<MemberDto> GetMemberByUsernameAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username.ToLower())
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync() 
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        #endregion
    }
}
