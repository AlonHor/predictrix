using Microsoft.EntityFrameworkCore;
using Predictrix.Domain.Models;

namespace Predictrix.Infrastructure
{
    public interface IApplicationDbContext
    {
        public DbSet<User> Users { get; }
        public DbSet<Prediction> Predictions { get; }
        public DbSet<Assertion> Assertions { get; }
        public DbSet<Chat> Chats { get; }
        public DbSet<Vote> Votes { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
