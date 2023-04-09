using PainterArm.Calibration;
using System.Text.Json;

namespace PainterCore.Configuration
{
    /// <summary>
    /// Class which is responsible for saving and loading configuration files
    /// </summary>
    static public class ConfigurationManager
    {
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true,
            IncludeFields = true,
            IgnoreReadOnlyFields = true
        };

        /// <summary>
        /// Saves variable of certain typename to file
        /// </summary>
        /// <typeparam name="T">Type of the object to save</typeparam>
        /// <param name="obj">Object to save</param>
        /// <param name="path">Path to configuration file</param>
        public static void SaveToFile<T>(T obj, string path)
        {
            string result = JsonSerializer.Serialize(obj, _serializerOptions);
            File.WriteAllText(path, result);
        }

        /// <summary>
        /// Saves variable of certain typename to file
        /// </summary>
        /// <typeparam name="T">Type of the object to save</typeparam>
        /// <param name="obj">Object to save</param>
        /// <param name="path">Path to configuration file</param>
        public static T? LoadFromFile<T>(string path)
        {
            string result = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(result, _serializerOptions);
        }

        /// <summary>
        /// Experimental function on loading and saving calibration settings of different devices
        /// </summary>
        /// <typeparam name="T">Data structure of device calibration configuration</typeparam>
        /// <param name="loadableObject">Object, where the calibration is being used</param>
        /// <param name="actionOnCalibrate">Function, which is invoked for calibration</param>
        /// <param name="configPath">Path to save file</param>
        /// <param name="configName">Headline of current dialog</param>
        public static void CalibrationDialog<T>(out T loadableObject, AbstractCalibrationBehavior calibrationBehavior, string configPath, string configName = "Calibration")
            where T : class, ICalibratable
        {
            Console.WriteLine($"---- [{configName}] ----");
            Console.WriteLine("Load previous configuration? [Y/N]");

            while (true)
            {
                Console.Write("> ");
                switch (Console.ReadLine())
                {
                    case "Y":
                        loadableObject = LoadFromFile<T>(configPath)!;
                        return;
                    case "N":
                        loadableObject = (calibrationBehavior.Calibrate() as T)!;
                        SaveToFile(loadableObject, configPath);
                        return;
                    default:
                        Console.WriteLine("Unknown response. Try again.");
                        break;
                }
                Console.WriteLine("\n");
            }
        }
    }
}
