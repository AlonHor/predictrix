using MediatR;
using Microsoft.EntityFrameworkCore;
using Predictrix.Domain.Models;
using Predictrix.Infrastructure;

namespace Predictrix.Application.Commands
{
    public class CreateOrUpdateUserCommand : IRequest<Result>
    {
        public required string UserId { get; init; }
        public required string Email { get; init; }
        public required string DisplayName { get; init; }
        public required string PhotoUrl { get; init; }
    }

    public class CreateOrUpdateUserCommandHandler(IApplicationDbContext dbContext)
        : IRequestHandler<CreateOrUpdateUserCommand, Result>
    {
        public async Task<Result> Handle(CreateOrUpdateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId,
                cancellationToken: cancellationToken);

            if (existingUser is null)
            {
                var newUser = new User
                {
                    UserId = request.UserId,
                    Email = request.Email,
                    PhotoUrl = request.PhotoUrl,
                    DisplayName = request.DisplayName,
                };

                dbContext.Users.Add(newUser);
            }
            else
            {
                existingUser.PhotoUrl = request.PhotoUrl;
                existingUser.DisplayName = request.DisplayName;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            
            return Result.Ok();
        }
    }
}