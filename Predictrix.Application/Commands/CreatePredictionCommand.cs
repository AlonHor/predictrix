using MediatR;
using Predictrix.Domain.Models;
using Predictrix.Infrastructure;

namespace Predictrix.Application.Commands
{
    public class CreatePredictionCommand : IRequest<Result>
    {
        public required Prediction Prediction { get; init; }
    }

    public class CreatePredictionCommandHandler(ApplicationDbContext dbContext)
        : IRequestHandler<CreatePredictionCommand, Result>
    {
        public async Task<Result> Handle(CreatePredictionCommand request, CancellationToken cancellationToken)
        {
            var prediction = request.Prediction;
            
            if (prediction.Confidence is < 0 or > 100)
                return Result.Fail("Confidence must be between 0 and 100!");

            var assertion = await dbContext.Assertions.FindAsync([prediction.AssertionId], cancellationToken: cancellationToken);

            if (assertion is null)
                return Result.Fail("Assertion not found");

            if (assertion.CastingForecastDeadline < DateTime.UtcNow)
                return Result.Fail("Assertion is no longer accepting predictions");
            
            var chat = await dbContext.Chats.FindAsync([assertion.ChatId], cancellationToken: cancellationToken);
            
            if (chat is null)
                return Result.Fail("Chat not found");
            
            if (chat.Members.All(m => m != assertion.UserId))
                return Result.Fail("User is not a member of the chat");
            
            if (dbContext.Predictions.Any(p => p.UserId == prediction.UserId && p.AssertionId == prediction.AssertionId))
                return Result.Fail("You have already made a prediction for this assertion");

            dbContext.Predictions.Add(prediction);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}