namespace JakaAPI
{
    /// <summary>
    /// A structure that represents internal command parameter of a Jaka Robot
    /// </summary>
    public struct CommandParameter
    {
        public string Key { get; private set; }

        public string Value { get; private set; }

        /// <summary>
        /// Constructor for a Command Parameter
        /// </summary>
        /// <param name="key">Name of a parameter</param>
        /// <param name="value">Value of a named parameter</param>
        /// <param name="quotes">Informs if a value should wrapped with quotes (string-like) [opt]</param>
        public CommandParameter(string key, string value, bool quotes = false)
        {
            Key = key;
            Value = value;
            if (quotes) Value = $"\"{Value}\"";
        }
    }

    public static class JakaCommand
    {
        /// <summary>
        /// Static method for building a JSON-like string as a full Jaka Robot command
        /// </summary>
        /// <param name="name">Main command name</param>
        /// <param name="parameters">Additional parameters to main command (e.g. speed, acceleration, etc.)</param>
        /// <returns>Jaka Robot internal command with specified options</returns>
        public static string Build(string name, params CommandParameter[] parameters)
        {
            string command = $"\"cmdName\":\"{name}\"";
            foreach (CommandParameter parameter in parameters)
            {
                command += $",\"{parameter.Key}\":{parameter.Value}";
            }
            return $"{{{command}}}";
        }

        public static byte[] BuildAsByteArray(string name, params CommandParameter[] parameters)
        {
            return System.Text.Encoding.ASCII.GetBytes(Build(name, parameters));
        }
    }
}
