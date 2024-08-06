namespace Predictrix.Domain.Requests
{
    public class CreatePredictionRequest
    {
        public required int AssertionId { get; init; }
        public required bool Forecast { get; init; }
        public string Rationale { get; init; } = string.Empty;
        public required int Confidence { get; init; }
    }
}