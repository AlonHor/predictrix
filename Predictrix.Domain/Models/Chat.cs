namespace Predictrix.Domain.Models;

public enum ChatType
{
    Private,
    Group
}

public class Chat
{
    public int Id { get; init; }
    public required ChatType Type { get; init; }
    
    public required ICollection<string> Members { get; set; }
    
    public string ScoreSumPerUser { get; set; } = string.Empty;
    public string PredictionsPerUser { get; set; } = string.Empty;
    
    public ICollection<int> Assertions { get; set; } = new List<int>();
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}