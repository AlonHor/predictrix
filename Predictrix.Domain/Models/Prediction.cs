namespace Predictrix.Domain.Models
{
    public class Prediction
    {
        public int Id { get; init; }
        public required string UserId { get; init; }
        public required int AssertionId { get; init; }
        public required bool Forecast { get; init; }
        public required int Confidence { get; init; }
        public string Rationale { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}
