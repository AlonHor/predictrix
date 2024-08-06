using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Predictrix.Infrastructure;
using Serilog;

namespace Predictrix.Application.Commands
{
    public class ResolveAssertionCommand : IRequest<Result>
    {
        public required int AssertionId { get; init; }
    }

    public class ResolveAssertionCommandHandler(ApplicationDbContext dbContext)
        : IRequestHandler<ResolveAssertionCommand, Result>
    {
        public async Task<Result> Handle(ResolveAssertionCommand request, CancellationToken cancellationToken)
        {
            var assertion = await dbContext.Assertions.FindAsync([request.AssertionId], cancellationToken: cancellationToken);
            
            if (assertion is null)
                return Result.Fail("Assertion not found");
            
            if (assertion.Completed)
                return Result.Fail("Assertion already resolved");
            
            var chat = await dbContext.Chats.FindAsync([assertion.ChatId], cancellationToken: cancellationToken);
            
            if (chat is null)
                return Result.Fail("Chat not found");
            
            var predictions = await dbContext.Predictions.Where(prediction => prediction.AssertionId == request.AssertionId).ToListAsync(cancellationToken);

            foreach (var userId in predictions.Select(prediction => prediction.UserId))
            {
                var user = await dbContext.Users.FindAsync([userId, cancellationToken], cancellationToken: cancellationToken);
                
                if (user is null)
                    return Result.Fail("User not found");

                var prediction = predictions.FirstOrDefault(prediction => prediction.UserId == userId);
                
                if (prediction is null)
                    return Result.Fail("Prediction not found");

                var normalizedPrediction = prediction.Forecast ? prediction.Confidence : 100 - prediction.Confidence;
                var correctness = assertion.FinalAnswer == prediction.Forecast ? 1 : -1;
                
                var score = correctness * Math.Abs(50 - normalizedPrediction) + 50;
                
                Log.Information("Normalized prediction for user {UserId} is {NormalizedPrediction}, correctness is {Correctness}, score is {Score}", userId, normalizedPrediction, correctness, score);
                
                user.Scores.Add(score);
                
                var chatScores = JsonConvert.DeserializeObject<Dictionary<string, int>>(chat.ScoreSumPerUser) ?? new Dictionary<string, int>();

                chatScores.TryAdd(userId, 0);
                
                chatScores[userId] += score;
                chat.ScoreSumPerUser = JsonConvert.SerializeObject(chatScores);
                
                var chatPredictions = JsonConvert.DeserializeObject<Dictionary<string, int>>(chat.PredictionsPerUser) ??
                                      new Dictionary<string, int>();

                chatPredictions.TryAdd(userId, 0);
                
                chatPredictions[userId]++;
                chat.PredictionsPerUser = JsonConvert.SerializeObject(chatPredictions);
            }

            assertion.Completed = true;
            
            await dbContext.SaveChangesAsync(cancellationToken);
            
            return Result.Ok();
        }
    }
}