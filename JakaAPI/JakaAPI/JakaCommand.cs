namespace JakaAPI
{

    public struct CommandParameter
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

        public CommandParameter(string key, string value, bool quotes = false)
        {
            Key = key;
            Value = value;
            if (quotes) Value = $"\"{Value}\"";
        }
    }

    public static class JakaCommand
    {
        public static string Build(string name, params CommandParameter[] parameters)
        {
            string command = $"\"cmdName\":\"{name}\"";
            foreach (var parameter in parameters)
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
