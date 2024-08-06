using MediatR;
using Microsoft.EntityFrameworkCore;
using Predictrix.Domain.Models;
using Predictrix.Infrastructure;

namespace Predictrix.Application.Queries;

public class GetPredictionsQuery : IRequest<Result<List<Prediction>>>
{
    public required string UserId { get; init; }
    public required int AssertionId { get; init; }
}

public class GetPredictionsQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetPredictionsQuery, Result<List<Prediction>>>
{
    public async Task<Result<List<Prediction>>> Handle(GetPredictionsQuery request, CancellationToken cancellationToken)
    {
        var assertion = await dbContext.Assertions.FindAsync([request.AssertionId], cancellationToken);
        if (assertion == null)
            return Result<List<Prediction>>.Fail("Assertion not found");
        
        var chat = await dbContext.Chats.FindAsync([assertion.ChatId], cancellationToken);
        if (chat == null || !chat.Members.Contains(request.UserId))
            return Result<List<Prediction>>.Fail("You are not a member of the chat this assertion belongs to");

        var predictions = await dbContext.Predictions.Where(prediction => prediction.AssertionId == request.AssertionId)
            .ToListAsync(cancellationToken);

        return Result<List<Prediction>>.Ok(predictions);
    }
}