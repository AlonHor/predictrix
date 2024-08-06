namespace Predictrix.Domain.Models
{
    public class Vote
    {
        public int Id { get; init; }
        public required int AssertionId { get; init; }
        public required string UserId { get; init; }
        public required bool Answer { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}
