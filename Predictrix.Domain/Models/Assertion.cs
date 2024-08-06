namespace Predictrix.Domain.Models
{
    public class Assertion
    {
        public int Id { get; init; }
        public required int ChatId { get; init; }
        public required string UserId { get; init; }
        public required string Text { get; init; }
        public bool FinalAnswer { get; set; }
        public bool Completed { get; set; } = false;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public required DateTime CastingForecastDeadline { get; init; }
        public required DateTime ValidationDate { get; init; }
    }
}
