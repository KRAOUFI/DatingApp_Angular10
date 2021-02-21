using System.Threading.Tasks;
using API.Repositories;

namespace API.Interfaces.IUnitOfWork
{
    public interface IUnitOfWork
    {
        UserRepository UserRepository { get; }
        MessagesRepository MessageRepository { get; }
        LikesRepository LikesRepository { get; }
        PhotoRepository PhotoRepository { get; }
        GroupRepository GroupRepository { get; }
        ConnectionRepository ConnectionRepository { get; }

        Task<bool> Complete();
        bool HasChanges();
    }
}