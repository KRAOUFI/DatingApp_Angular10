using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces.IServices;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessagesService _messageService;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _presenceTracker;
        private readonly UserRepository _userRepo;

        public MessageHub(
            IMessagesService messageService,
            IMapper mapper,
            IHubContext<PresenceHub> presenceHub,
            PresenceTracker presenceTracker,
            UserRepository userRepo)
        {
            _mapper = mapper;
            _messageService = messageService;
            _presenceHub = presenceHub;
            _presenceTracker = presenceTracker;
            _userRepo = userRepo;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await _messageService.GetMessageThread(Context.User.GetUserName(), otherUser);
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(ex);
        }

        public async Task SendMessage(CreateMessageDto dto)
        {
            try
            {
                var messageToAdd = new Message();
                var sender = await _userRepo.GetUserByUsernameAsync(Context.User.GetUserName());

                var groupName = GetGroupName(sender.UserName, dto.RecipientUserName);

                var group = await _messageService.GetMessageGroup(groupName);
                if (group.Connections.Any(x => x.Username == dto.RecipientUserName))
                {
                    messageToAdd.DateRead = DateTime.UtcNow;
                }
                else 
                {
                    var connections = await _presenceTracker.GetConnectionsForUser(dto.RecipientUserName);
                    if(connections != null)
                    {
                        await _presenceHub.Clients
                        .Clients(connections)
                        .SendAsync("NewMessageReceived", 
                            new {username = sender.UserName, knownAs = sender.KnownAs });
                    }
                }

                var messageDto = await _messageService.CreateMessage(Context.User.GetUserName(), dto, messageToAdd);

                await Clients.Group(groupName).SendAsync("NewMessage", messageDto);
            }
            catch (Exception ex)
            {
                throw new HubException(ex.Message);
            }
        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _messageService.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());
            if (group == null)
            {
                group = new Group(groupName);
                await _messageService.AddGroup(group);
            }

            return await _messageService.AddConnectionn(connection) > 0 ? 
                    group : 
                    throw new HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageService.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            return await _messageService.RemoveConnection(connection) > 0 ? 
                    group : 
                    throw new HubException("Failed to remove from group");
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.Compare(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}