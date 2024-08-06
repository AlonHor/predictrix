using MediatR;
using Microsoft.EntityFrameworkCore;
using Predictrix.Domain.Models;
using Predictrix.Infrastructure;

namespace Predictrix.Application.Queries;

public class GetChatsQuery: IRequest<Result<IEnumerable<Chat>>>
{
    public required string UserId { get; init; }
}

public class GetChatsQueryHandler(IApplicationDbContext dbContext) : IRequestHandler<GetChatsQuery, Result<IEnumerable<Chat>>>
{
    public async Task<Result<IEnumerable<Chat>>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        var chats = await dbContext.Chats.ToListAsync(cancellationToken);
        return Result<IEnumerable<Chat>>.Ok(chats.Where(c => c.Members.Contains(request.UserId)).ToList());
    }
}