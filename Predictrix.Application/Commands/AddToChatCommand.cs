using MediatR;
using Predictrix.Domain.Models;
using Predictrix.Infrastructure;

namespace Predictrix.Application.Commands
{
    public class AddToChatCommand : IRequest<Result>
    {
        public required string UserId { get; init; }
        public required string NewUserId { get; init; }
        public required int ChatId { get; init; }
    }

    public class AddToChatCommandHandler(ApplicationDbContext dbContext)
        : IRequestHandler<AddToChatCommand, Result>
    {
        public async Task<Result> Handle(AddToChatCommand request, CancellationToken cancellationToken)
        {
            var chat = await dbContext.Chats.FindAsync([request.ChatId], cancellationToken: cancellationToken);
            
            if (chat is null)
                return Result.Fail("Chat not found");
            
            if (chat.Members.All(m => m != request.UserId))
                return Result.Fail("You are not a member of this chat");

            var newMember = await dbContext.Users.FindAsync([request.NewUserId], cancellationToken: cancellationToken);

            if (newMember is null)
                return Result.Fail("User not found");
            
            if (chat.Members.Contains(newMember.UserId))
                return Result.Fail("User is already a member of this chat");

            if (chat.Type == ChatType.Private)
            {
                var newChat = new Chat
                {
                    Type = ChatType.Group,
                    Members = chat.Members.Append(newMember.UserId).ToList()
                };
                
                dbContext.Chats.Add(newChat);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            else
            {
                chat.Members.Add(newMember.UserId);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            
            return Result.Ok();
        }
    }
}