using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.DTOs;
using API.Helpers;

namespace API.Interfaces.IServices
{
    public interface IMessagesService
    {
        Task<int> AddMessage(Message message);
        Task<int> DeleteMessage(string currentUsername, int messageId);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recepientUsername);
        Task<MessageDto> CreateMessage(string senderUsername, CreateMessageDto dto);
    }
}
