namespace Predictrix.Domain.Requests
{
    public class CreateAssertionRequest
    {
        public required int ChatId { get; init; }
        public required string Text { get; init; }
        public required DateTime CastingForecastDeadline { get; init; }
        public required DateTime ValidationDate { get; init; }
    }
}