using System.Threading.Tasks;
using API.Interfaces.IUnitOfWork;
using API.Repositories;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public UserRepository UserRepository => new UserRepository(_context, _mapper);

        public MessagesRepository MessageRepository => new MessagesRepository(_context, _mapper);

        public LikesRepository LikesRepository => new LikesRepository(_context, _mapper);

        public PhotoRepository PhotoRepository => new PhotoRepository(_context, _mapper);

        public GroupRepository GroupRepository => new GroupRepository(_context);

        public ConnectionRepository ConnectionRepository => new ConnectionRepository(_context);

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
