using API.Data;
using API.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        public readonly DbSet<T> myEntity;
        private readonly DataContext _context;        

        public BaseRepository(DataContext context)
        {
            _context = context;
            myEntity = _context.Set<T>();
        }

        public async Task<List<T>> ToListAsync()
        {
            return await myEntity.ToListAsync();
        }

        public async Task<T> FindByIdAsync(int id)
        {
            return await myEntity.FindAsync(id);
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> where)
        {
            return await myEntity.Where(where).ToListAsync();
        }

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> where)
        {
            return await myEntity.AnyAsync(where);
        }

        public async Task<T> CreateAsync(T entity)
        {
            myEntity.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> DeleteAsync(int id)
        {
            var tt = await myEntity.FindAsync(id);
            if (tt == null)
            {
                return tt;
            }

            myEntity.Remove(tt);
            await _context.SaveChangesAsync();

            return tt;
        }        
    }
}
