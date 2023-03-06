using System.Globalization;

namespace PainterCore
{
    public struct CommandHPGL
    {
        public CodeHPGL Code { get; private set; }
        public double[] Arguments { get; private set; }

        public CommandHPGL(CodeHPGL code, double[] arguments)
        {
            Code = code;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return Code.ToString() + "[" + String.Join(",", Arguments.Select(p => p.ToString()).ToArray()) + "]";
        }
    }

    public enum CodeHPGL
    {
        IN, // Initialize, start a plotting job
        PC, // Pen color (x,r,g,b)
        PW, // Pen width (w,x)
        PU, // Pen up
        PD, // Pen down
        NP, // Number of pens
    }

    public class ParserHPGL
    {
        private readonly string _filePath;

        public ParserHPGL(string filePath = @"..\..\..\Resources\strokes.plt")
        {
            _filePath = filePath;
        }

        public IEnumerable<CommandHPGL> GetNextCommand()
        {
            using (StreamReader reader = new StreamReader(_filePath))
            {
                string buffer = "";

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] commands = line.Split(';');

                    if (buffer.Length > 0)
                    {
                        commands[0] = buffer + commands[0];
                        buffer = "";
                    }

                    if (line.EndsWith(";") == false)
                    {
                        buffer = commands[commands.Length - 1];
                        Array.Resize(ref commands, commands.Length - 1);
                    }

                    foreach (string command in commands)
                    {
                        yield return ParseCommand(command);
                    }
                }

                if (buffer.Length > 0)
                {
                    yield return ParseCommand(buffer);
                }
            }
        }

        private static CommandHPGL ParseCommand(string commandStr)
        {
            string codeStr = commandStr.Substring(0, 2);
            CodeHPGL code = (CodeHPGL)Enum.Parse(typeof(CodeHPGL), codeStr);

            string argsStr = commandStr.Substring(2);
            if (argsStr.Length == 0)
            {
                return new CommandHPGL(code, Array.Empty<double>());
            }
            string[] argsArr = argsStr.Split(',');
        
            double[] arguments = new double[argsArr.Length];
            for (int i = 0; i < argsArr.Length; i++)
            {
                arguments[i] = Double.Parse(argsArr[i], CultureInfo.InvariantCulture);
            }

            return new CommandHPGL(code, arguments);
        }
    }
}
