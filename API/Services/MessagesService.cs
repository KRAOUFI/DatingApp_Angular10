using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces.IServices;
using API.Interfaces.IUnitOfWork;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MessagesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> AddMessage(Message message)
        {
            _unitOfWork.MessageRepository.Add(message);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> DeleteMessage(string currentUsername, int messageId)
        {
            try
            {
                var messageQuery = _unitOfWork.MessageRepository.AsQueryable();
                var message = await messageQuery
                    .Include(x => x.Sender)
                    .Include(x => x.Recipient)
                    .FirstOrDefaultAsync(x => x.Id == messageId);

                if (message == null)
                    throw new Exception("notfound");

                if (message.Sender.UserName != currentUsername && message.Recipient.UserName != currentUsername)
                    throw new Exception("unautorized");

                if (message.Sender.UserName == currentUsername)
                    message.SenderDeleted = true;

                if (message.Recipient.UserName == currentUsername)
                    message.RecepientDeleted = true;

                var valToReturn = await _unitOfWork.Complete();

                // La suppressions physique du message se fait uniquement quand le 2 ont demandé la suppression du message
                if (message.RecepientDeleted && message.SenderDeleted)
                    await _unitOfWork.MessageRepository.DeleteAsync(messageId);

                return valToReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _unitOfWork.MessageRepository.GetByIdAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _unitOfWork.MessageRepository.AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => 
                    u.RecipientUsername == messageParams.Username && u.RecepientDeleted == false),
                "Outbox" => query.Where(u => 
                    u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u =>
                    u.RecipientUsername == messageParams.Username &&
                    u.RecepientDeleted == false &&
                    u.DateRead == null)
            };

            var queryMessages = query
                .OrderByDescending(m => m.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(queryMessages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recepientUsername)
        {
            var messagesQuery = _unitOfWork.MessageRepository.AsQueryable();
            var messages = await messagesQuery
                //.Include(u => u.Sender).ThenInclude(p => p.Photos)    // Avec ProjectTo nous n'avons plus besoin 
                //.Include(u => u.Recipient).ThenInclude(p => p.Photos) // de faire des Include
                .Where(x =>
                    x.Recipient.UserName == currentUsername &&
                    x.Sender.UserName == recepientUsername &&
                    x.RecepientDeleted == false
                    ||
                    x.Recipient.UserName == recepientUsername &&
                    x.Sender.UserName == currentUsername &&
                    x.SenderDeleted == false)
                .OrderBy(m => m.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)  //Pour optimiser les requetes sql générés
                .ToListAsync();

            var unreadMessages = messages
            .Where(m => m.DateRead == null && m.RecipientUsername == currentUsername)
            .ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            return messages;
        }

        public async Task<MessageDto> CreateMessage(string senderUsername, CreateMessageDto dto, Message messageToAdd)
        {
            try
            {
                if (senderUsername == dto.RecipientUserName.ToLower())
                    throw new Exception("You can not send a message to you self");

                var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(senderUsername);
                var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(dto.RecipientUserName);

                if (recipient == null)
                    throw new Exception("Cannot find user");

                if (messageToAdd == null)
                {
                    messageToAdd = new Message()
                    {
                        Content = dto.Content,
                        Sender = sender,
                        SenderUsername = sender.UserName,
                        Recipient = recipient,
                        RecipientUsername = recipient.UserName
                    };

                }
                else
                {
                    messageToAdd.Content = dto.Content;
                    messageToAdd.Sender = sender;
                    messageToAdd.SenderUsername = sender.UserName;
                    messageToAdd.Recipient = recipient;
                    messageToAdd.RecipientUsername = recipient.UserName;
                }

                _unitOfWork.MessageRepository.Add(messageToAdd);
                if (await _unitOfWork.Complete())
                    return _mapper.Map<MessageDto>(messageToAdd);

                throw new Exception("Failed to send message");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddGroup(Group group)
        {
            _unitOfWork.GroupRepository.Add(group);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> AddConnectionn(Connection connection)
        {
            _unitOfWork.ConnectionRepository.Add(connection);
            return await _unitOfWork.Complete();
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _unitOfWork.ConnectionRepository.GetByConditionAsync(x => x.ConnectionId == connectionId);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            var groupQuery = _unitOfWork.GroupRepository.AsQueryable();

            return await groupQuery
                            .Include(x => x.Connections)
                            .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<bool> RemoveConnection(Connection connection)
        {
            _unitOfWork.ConnectionRepository.Remove(connection);
            return await _unitOfWork.Complete();
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            var groupQuery = _unitOfWork.GroupRepository.AsQueryable();
            return await groupQuery
                        .Include(g => g.Connections)
                        .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                        .FirstOrDefaultAsync();
        }
    }
}
