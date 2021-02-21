using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessagesService _messageService;

        public MessagesController(IMessagesService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto dto) 
        {
            try 
            {
                var messageDto = await _messageService.CreateMessage(User.GetUserName(), dto, null);
                return Ok(messageDto);
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();
            var messages = await _messageService.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(messages.CurrentPage,messages.PageSize, messages.TotalCount, messages.TotalPages);
            return messages;
        }

        /* 
         ************************* Cette fonction a été déplacé dans SignalR *************************
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesThread(string username)
        {
            var currentUsername = User.GetUserName();
            return Ok(await _messageService.GetMessageThread(currentUsername, username));
        }
        */

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id) 
        {
            try
            {
                if(await _messageService.DeleteMessage(User.GetUserName(), id))
                    return Ok();
                else
                    return BadRequest("Problem deleting the message");
            }
            catch(Exception ex)
            {
                switch(ex.Message)
                {
                    case "unautorized":
                        return Unauthorized();
                    default:
                        return NotFound();
                }
            }
        }
    }
}
