namespace Predictrix.Domain.Models
{
    public class User
    {
        public required string UserId { get; init; }
        public required string Email { get; init; }
        
        public required string DisplayName { get; set; }
        public required string PhotoUrl { get; set; }

        public ICollection<int> ScoreSum { get; set; } = new List<int>();
        
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}
