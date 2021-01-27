using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces.IServices;
using API.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly MessagesRepository _messagesRepo;
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepo;
        
        public MessagesService(
            UserRepository userRepo, 
            MessagesRepository messagesRepo, 
            IMapper mapper)
        {
            _messagesRepo = messagesRepo;
            _mapper = mapper;
            _userRepo = userRepo;
        }

        public async Task<int> AddMessage(Message message)
        {
            _messagesRepo.Add(message);
            return await _messagesRepo.SaveAsync();
        }

        public async Task<int> DeleteMessage(string currentUsername, int messageId)
        {
            try
            {
                var messageQuery = _messagesRepo.AsQueryable();
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
                
                var valToReturn = await _messagesRepo.SaveAsync();

                // La suppressions physique du message se fait uniquement quand le 2 ont demandé la suppression du message
                if (message.RecepientDeleted && message.SenderDeleted)
                    await _messagesRepo.DeleteAsync(messageId);

                return valToReturn;
            } catch(Exception ex) 
            {
                throw ex;
            }            
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _messagesRepo.GetByIdAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _messagesRepo.AsQueryable();
            query = messageParams.Container switch 
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username && u.RecepientDeleted == false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => 
                    u.Recipient.UserName == messageParams.Username && 
                    u.RecepientDeleted == false &&
                    u.DateRead == null) // Messages not yet readen
            };

            var messages = query
                .OrderByDescending(m => m.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recepientUsername)
        {
            var messagesQuery = _messagesRepo.AsQueryable();
            var messages = await messagesQuery
                .Include(u => u.Sender).ThenInclude(p =>p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(x => 
                    x.Recipient.UserName == currentUsername &&
                    x.Sender.UserName == recepientUsername && 
                    x.RecepientDeleted == false
                    || 
                    x.Recipient.UserName == recepientUsername &&
                    x.Sender.UserName == currentUsername &&
                    x.SenderDeleted == false)
                .OrderBy(m => m.MessageSent)
                .ToListAsync();
            
            var unreadMessages = messages
            .Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername)
            .ToList();

            if(unreadMessages.Any()) 
            {
                foreach(var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                    _messagesRepo.Update(message);
                    await _messagesRepo.SaveAsync();
                }                
            } 

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<MessageDto> CreateMessage(string senderUsername, CreateMessageDto dto) 
        {
            try 
            {
                if (senderUsername == dto.RecipientUserName.ToLower())
                    throw new Exception("You can not send a message to you self");
                
                var sender = await _userRepo.GetUserByUsernameAsync(senderUsername);
                var recipient = await _userRepo.GetUserByUsernameAsync(dto.RecipientUserName);
                
                if (recipient == null)
                    throw new Exception("Cannot find user");
                
                var message = new Message() 
                {
                    Content = dto.Content,
                    Sender = sender,
                    SenderUsername = sender.UserName,
                    Recipient = recipient,                    
                    RecipientUsername = recipient.UserName
                };
                
                _messagesRepo.Add(message);
                if (await _messagesRepo.SaveAsync() > 0)
                    return _mapper.Map<MessageDto>(message);
                
                throw new Exception("Failed to send message");
            }
            catch (Exception ex) 
            {
                throw ex;
            }
            
        }
    }
    
}
