using System.Text.Json;

namespace PainterCore.Configuration
{
    static public class ConfigurationManager
    {
        public static void SaveToFile<T>(T obj, string path)
        {
            FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.Write);
            JsonSerializer.Serialize(fs, obj);
        }

        public static T? LoadFromFile<T>(string path)
        {
            FileStream fs = new(path, FileMode.Open, FileAccess.Read);
            return JsonSerializer.Deserialize<T>(fs);
        }
    }
}
