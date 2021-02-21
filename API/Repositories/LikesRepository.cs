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

        public void Add(Like entity)
        {
            _entity.Add(entity);
        }

        public void Update(Like entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            var entityToDelete = await _entity.FindAsync(id);
            if (entityToDelete != null) 
            {
                _entity.Remove(entityToDelete);
            }
        }

        public IQueryable<Like> AsQueryable() 
        {
            return _entity.AsQueryable();
        }
    }
}