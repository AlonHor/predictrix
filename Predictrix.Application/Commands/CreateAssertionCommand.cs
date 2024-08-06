using MediatR;
using Predictrix.Domain.Models;
using Predictrix.Infrastructure;

namespace Predictrix.Application.Commands
{
    public class CreateAssertionCommand : IRequest<Result>
    {
        public required Assertion Assertion { get; init; }
    }

    public class CreateAssertionCommandHandler(ApplicationDbContext dbContext)
        : IRequestHandler<CreateAssertionCommand, Result>
    {
        public async Task<Result> Handle(CreateAssertionCommand request, CancellationToken cancellationToken)
        {
            var assertion = request.Assertion;

            var chat = await dbContext.Chats.FindAsync([assertion.ChatId], cancellationToken: cancellationToken);
            
            if (chat is null)
                return Result.Fail("Chat not found");
            
            if (chat.Members.All(m => m != assertion.UserId))
                return Result.Fail("You are not a member of this chat");

            if (assertion.CastingForecastDeadline > assertion.ValidationDate)
                return Result.Fail("Assertion's casting forecast deadline cannot be after the validation date");
            
            if (assertion.CastingForecastDeadline < DateTime.UtcNow)
                return Result.Fail("Assertion's casting forecast deadline cannot be in the past");
            
            dbContext.Assertions.Add(assertion);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}