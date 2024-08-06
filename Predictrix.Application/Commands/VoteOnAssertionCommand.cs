using MediatR;
using Microsoft.EntityFrameworkCore;
using Predictrix.Domain.Models;
using Predictrix.Infrastructure;

namespace Predictrix.Application.Commands
{
    public class VoteOnAssertionCommand : IRequest<Result>
    {
        public required int AssertionId { get; init; }
        public required string UserId { get; init; }
        public required bool Answer { get; init; }
    }

    public class VoteOnAssertionCommandHandler(ApplicationDbContext dbContext, ISender mediator)
        : IRequestHandler<VoteOnAssertionCommand, Result>
    {
        public async Task<Result> Handle(VoteOnAssertionCommand request, CancellationToken cancellationToken)
        {
            var assertion = await dbContext.Assertions.FindAsync([request.AssertionId], cancellationToken: cancellationToken);
            
            if (assertion is null)
                return Result.Fail("Assertion not found");
            
            if (!dbContext.Predictions.Any(prediction => prediction.AssertionId == request.AssertionId && prediction.UserId == request.UserId))
                return Result.Fail("You cannot vote on an assertion you have not predicted");
            
            if (assertion.ValidationDate > DateTime.UtcNow)
                return Result.Fail("This assertion does not require validation yet");
            
            var chat = await dbContext.Chats.FindAsync([assertion.ChatId], cancellationToken: cancellationToken);
            
            if (chat is null || !chat.Members.Contains(request.UserId))
                return Result.Fail("You are not a member of the chat this assertion belongs to");
            
            var existingVote = await dbContext.Votes
                .Where(vote => vote.AssertionId == request.AssertionId && vote.UserId == request.UserId)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (existingVote is not null)
                return Result.Fail("You have already voted on this assertion");

            var vote = new Vote
            {
                Answer = request.Answer,
                AssertionId = request.AssertionId,
                UserId = request.UserId
            };
            
            dbContext.Votes.Add(vote);

            var shouldResolve = false;
            if (dbContext.Predictions.Count(p => p.AssertionId == request.AssertionId) == dbContext.Votes.Count(v => v.AssertionId == request.AssertionId) + 1)
            {
                shouldResolve = true;
                
                var votes = await dbContext.Votes
                    .Where(v => v.AssertionId == request.AssertionId)
                    .ToListAsync(cancellationToken);
                
                var yesVotes = votes.Count(v => v.Answer);
                var noVotes = votes.Count - yesVotes;
                
                assertion.FinalAnswer = yesVotes > noVotes;
                
            }
            
            await dbContext.SaveChangesAsync(cancellationToken); // SAVE AFTER!
            
            if (shouldResolve)
                return await mediator.Send(new ResolveAssertionCommand
                {
                    AssertionId = request.AssertionId,
                }, cancellationToken);
            
            return Result.Ok();
        }
    }
}