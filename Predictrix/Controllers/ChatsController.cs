using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Predictrix.Application.Commands;
using Predictrix.Application.Queries;
using Predictrix.Domain.Models;
using Predictrix.Domain.Requests;
using Predictrix.Services;
using Serilog;

namespace Predictrix.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController(ISender mediator, IUserService userService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request)
        {
            var userId = userService.GetUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var chat = new Chat
            {
                Type = request.Type,
                Members = request.Members
            };
            
            Log.Information("Creating chat for users {Members}", string.Join(", ", [..chat.Members, userId]));

            var result = await mediator.Send(new CreateChatCommand
            {
                UserId = userId,
                Chat = chat
            });
            
            if (result.IsFail)
                return BadRequest(result.Message);
            
            return Ok();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetChats()
        {
            var userId = userService.GetUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            Log.Information("Retrieving chats for user {UserId}", userId);
            
            var result = await mediator.Send(new GetChatsQuery
            {
                UserId = userId
            });
            
            if (result.IsFail)
                return BadRequest(result.Message);

            var chats = result.Data;
            
            if (chats == null)
                throw new Exception("Chat not found");

            return Ok(chats.Select(c => new
            {
                c.Id,
                c.Type,
                Members = c.Members.Where(m => m != userId)
            }));
        }

        [HttpGet("invite")]
        public IActionResult InviteToChat()
        {
            var userId = userService.GetUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            return Ok(userId);
        }
        
        [HttpPost("{id:int}/add")]
        public async Task<IActionResult> AddToChat(int id, [FromBody] AddToChatRequest request)
        {
            var userId = userService.GetUserId();
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
        
            var result = await mediator.Send(new AddToChatCommand
            {
                UserId = userId,
                NewUserId = request.UserId,
                ChatId = id
            });
            
            if (result.IsFail)
                return BadRequest(result.Message);
            
            return Ok();
        }
    }
}
