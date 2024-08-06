using MediatR;
using Predictrix.Domain.Models;
using Predictrix.Infrastructure;

namespace Predictrix.Application.Queries;

public class GetAssertionQuery: IRequest<Result<Assertion>>
{
    public required string UserId { get; init; }
    public required int AssertionId { get; init; }
}

public class GetAssertionQueryHandler(IApplicationDbContext dbContext) : IRequestHandler<GetAssertionQuery, Result<Assertion>>
{
    public async Task<Result<Assertion>> Handle(GetAssertionQuery request, CancellationToken cancellationToken)
    {
        var assertion = await dbContext.Assertions.FindAsync([request.AssertionId], cancellationToken);
        
        if (assertion == null)
            return Result<Assertion>.Fail("Assertion not found");
        
        var chat = await dbContext.Chats.FindAsync([assertion.ChatId], cancellationToken);
        if (chat == null || !chat.Members.Contains(request.UserId))
            return Result<Assertion>.Fail("You are not a member of the chat this assertion belongs to");
        
        return Result<Assertion>.Ok(assertion);
    }
}