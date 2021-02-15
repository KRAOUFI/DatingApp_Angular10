using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class GroupRepository : IRepository<Group>
    {
        private readonly DataContext _context;        
        private readonly DbSet<Group> _entity;

        public GroupRepository(DataContext context)
        {
            _context = context;
            _entity = _context.Set<Group>();
        }

        public async Task<Group> GetByIdAsync(int id)
        {
            return await _entity.FindAsync(id);
        }

        public async Task<Group> GetByConditionAsync(Expression<Func<Group, bool>> where)
        {
            return await _entity.SingleOrDefaultAsync(where);
        }

        public async Task<IEnumerable<Group>> GetAllAsync()
        {
            return await _entity.ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetAllByConditionAsync(Expression<Func<Group, bool>> where)
        {
            return await _entity.Where(where).ToListAsync();
        }

        public IQueryable<Group> AsQueryable()
        {
            return _entity.AsQueryable();
        }

        public async Task<bool> ExistAsync(Expression<Func<Group, bool>> where)
        {
            return await _entity.AnyAsync(where);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Add(Group entity)
        {
            _entity.Add(entity);
        }
        public void Update(Group entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            var entityToDelete = await this.GetByIdAsync(id);
            if (entityToDelete != null)
            {
                _entity.Remove(entityToDelete);
            }
        }
    }
}