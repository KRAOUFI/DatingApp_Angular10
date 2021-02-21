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
    public class ConnectionRepository : IRepository<Connection>
    {
        private readonly DataContext _context;
        private readonly DbSet<Connection> _entity;
        public ConnectionRepository(DataContext context)
        {
            _context = context;
            _entity = _context.Set<Connection>();
        }

        public async Task<Connection> GetByIdAsync(int id)
        {
            return await _entity.FindAsync(id);
        }

        public async Task<Connection> GetByConditionAsync(Expression<System.Func<Connection, bool>> where)
        {
            return await _entity.SingleOrDefaultAsync(where);
        }

        public async Task<IEnumerable<Connection>> GetAllAsync()
        {
            return await _entity.ToListAsync();
        }

        public async Task<IEnumerable<Connection>> GetAllByConditionAsync(Expression<System.Func<Connection, bool>> where)
        {
            return await _entity.Where(where).ToListAsync();
        }

        public IQueryable<Connection> AsQueryable()
        {
            return _entity.AsQueryable();
        }

        public async Task<bool> ExistAsync(Expression<System.Func<Connection, bool>> where)
        {
            return await _entity.AnyAsync(where);
        }

        public void Add(Connection entity)
        {
            _entity.Add(entity);
        }

        public void Update(Connection entity)
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

        public void Remove(Connection connection)
        {
            _entity.Remove(connection);
        }
    }
}