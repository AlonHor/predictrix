using System.Text.Json;

namespace Predictrix.Services
{
    public interface IDictionarySerializerService
    {
        public string Serialize<T>(T dictionary);
        public T? Deserialize<T>(string dictionary);
    }
    
    public class DictionarySerializerService : IDictionarySerializerService
    {
        public string Serialize<T>(T dictionary)
        {
            return JsonSerializer.Serialize(dictionary);
        }

        public T? Deserialize<T>(string dictionary)
        {
            return JsonSerializer.Deserialize<T>(dictionary);
        }
    }
}