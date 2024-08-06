using MediatR;
using Microsoft.EntityFrameworkCore;
using Predictrix.Domain.Models;
using Predictrix.Infrastructure;

namespace Predictrix.Application.Queries;

public class GetAssertionsQuery : IRequest<Result<List<Assertion>>>
{
    public required string UserId { get; init; }
    public required int ChatId { get; init; }
}

public class GetAssertionsQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetAssertionsQuery, Result<List<Assertion>>>
{
    public async Task<Result<List<Assertion>>> Handle(GetAssertionsQuery request, CancellationToken cancellationToken)
    {
        var chat = await dbContext.Chats.FindAsync([request.ChatId], cancellationToken);
        if (chat == null || !chat.Members.Contains(request.UserId))
            return Result<List<Assertion>>.Fail("You are not a member of the chat this assertion belongs to");

        var assertions = await dbContext.Assertions.Where(assertion => assertion.ChatId == request.ChatId)
            .ToListAsync(cancellationToken);

        return Result<List<Assertion>>.Ok(assertions);
    }
}