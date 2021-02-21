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
    public class PhotoRepository : IRepository<Photo>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly DbSet<Photo> _entity;

        public PhotoRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _entity = _context.Set<Photo>();
        }

        public async Task<Photo> GetByIdAsync(int id)
        {
            return await _entity.FindAsync(id);
        }
        
        public async Task<Photo> GetByConditionAsync(Expression<Func<Photo, bool>> where)
        {
            return await _entity.SingleOrDefaultAsync(where);
        }

        public async Task<IEnumerable<Photo>> GetAllAsync()
        {
            return await _entity.ToListAsync();
        }

        public async Task<IEnumerable<Photo>> GetAllByConditionAsync(Expression<Func<Photo, bool>> where)
        {
            return await _entity.Where(where).ToListAsync();
        }

        public async Task<bool> ExistAsync(Expression<Func<Photo, bool>> where)
        {
            return await _entity.AnyAsync(where);
        }

        public void Add(Photo entity)
        {
            _entity.Add(entity);
        }

        public void Update(Photo entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            var photoToDelete = await _entity.FindAsync(id);
            if (photoToDelete != null)
            {
                _entity.Remove(photoToDelete);
            }
        }

        public IQueryable<Photo> AsQueryable() 
        {
            return _entity.AsQueryable();
        }
    }
}