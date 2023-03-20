using System.Text.Json;

namespace PainterCore.Configuration
{
    static public class ConfigurationManager
    {
        private static JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true,
            IncludeFields = true
        };

        public static void SaveToFile<T>(T obj, string path)
        {
            string result = JsonSerializer.Serialize(obj, _serializerOptions);
            File.WriteAllText(path, result);
        }

        public static T? LoadFromFile<T>(string path)
        {
            string result = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(result);
        }
    }
}
