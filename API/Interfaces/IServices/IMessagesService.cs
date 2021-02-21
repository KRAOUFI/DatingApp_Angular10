using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.DTOs;
using API.Helpers;

namespace API.Interfaces.IServices
{
    public interface IMessagesService
    {
        Task<bool> AddGroup(Group group);
        Task<bool> AddConnectionn(Connection connection);
        Task<bool> RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);
        Task<bool> AddMessage(Message message);
        Task<bool> DeleteMessage(string currentUsername, int messageId);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recepientUsername);
        Task<MessageDto> CreateMessage(string senderUsername, CreateMessageDto dto, Message messageToAdd);
    }
}
