using MediatR;
using Microsoft.EntityFrameworkCore;
using Predictrix.Domain.Models;
using Predictrix.Infrastructure;

namespace Predictrix.Application.Commands
{
    public class CreateChatCommand : IRequest<Result>
    {
        public required string UserId { get; init; }
        public required Chat Chat { get; init; }
    }

    public class CreateChatCommandHandler(ApplicationDbContext dbContext)
        : IRequestHandler<CreateChatCommand, Result>
    {
        public async Task<Result> Handle(CreateChatCommand request, CancellationToken cancellationToken)
        {
            var chat = request.Chat;

            var members = await dbContext.Users.Where(u => chat.Members.Contains(u.UserId)).ToListAsync(cancellationToken);
            
            if (members.Count != chat.Members.Count)
                return Result.Fail("One or more users not found");

            if (members.All(u => u.UserId != request.UserId))
            {
                var user = await dbContext.Users.FindAsync([request.UserId], cancellationToken: cancellationToken);
                if (user is null)
                    return Result.Fail("User not found");
                
                members.Add(user);
            }
            
            if (members.Count < 2)
                return Result.Fail("Chat must have at least 2 members");
            
            if (chat.Type == ChatType.Private && members.Count != 2)
                return Result.Fail("Private chat must have exactly 2 members");
            
            if (dbContext.Chats.Any(c => c.Members == members.Select(u => u.UserId)))
                return Result.Fail("Chat already exists");
            
            chat.Members = members.Select(u => u.UserId).ToList();

            dbContext.Chats.Add(chat);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}