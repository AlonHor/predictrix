using Predictrix.Domain.Models;

namespace Predictrix.Domain.Requests
{
    public class CreateChatRequest
    {
        public required ChatType Type { get; init; }
        public required IList<string> Members { get; set; }
    }
}