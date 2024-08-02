using Microsoft.EntityFrameworkCore;
using Predictrix.Domain.Models;

namespace Predictrix.Infrastructure
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Prediction> Predictions => Set<Prediction>();
        public DbSet<Assertion> Assertions => Set<Assertion>();
        public DbSet<Chat> Chats => Set<Chat>();
    }
}
