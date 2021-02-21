using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<T> GetByConditionAsync(Expression<Func<T, bool>> where);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllByConditionAsync(Expression<Func<T, bool>> where);        
        Task<bool> ExistAsync(Expression<Func<T, bool>> where);
        void Add(T entity);
        void Update(T entity);
        Task DeleteAsync(int id);        
        IQueryable<T> AsQueryable();
    }
}
