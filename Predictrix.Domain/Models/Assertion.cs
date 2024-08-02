namespace Predictrix.Domain.Models
{
    public class Assertion
    {
        public int Id { get; init; }
        public ICollection<int> Predictions { get; set; } = new List<int>();

        public required string Text { get; init; }
        public bool FinalAnswer { get; set; }

        public DateTime CreatedAt { get; init; }
        public required DateTime EndsAt { get; init; }
    }
}
