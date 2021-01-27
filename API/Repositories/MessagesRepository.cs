using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.IRepositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class MessagesRepository : IRepository<Message>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly DbSet<Message> _entity;
        public MessagesRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _entity = _context.Set<Message>();
        }

        public async Task<Message> GetByIdAsync(int id)
        {
            return await _entity.FindAsync(id);
        }

        public async Task<Message> GetByConditionAsync(Expression<Func<Message, bool>> where)
        {
            return await _entity.SingleOrDefaultAsync(where);
        }

        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            return await _entity.ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetAllByConditionAsync(Expression<Func<Message, bool>> where)
        {
            return await _entity.Where(where).ToListAsync();
        }

        public async Task<bool> ExistAsync(Expression<Func<Message, bool>> where)
        {
            return await _entity.AnyAsync(where);
        }

        public void Add(Message entity)
        {
            _entity.Add(entity);
        }

        public void Update(Message entity)
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

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IQueryable<Message> AsQueryable() 
        {
            return _entity.AsQueryable();
        }
    }
}
