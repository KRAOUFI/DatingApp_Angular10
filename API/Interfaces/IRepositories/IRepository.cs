using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> ToListAsync();
        Task<T> FindByIdAsync(int id);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> where);
        Task<bool> ExistAsync(Expression<Func<T, bool>> where);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(int id);
    }
}
