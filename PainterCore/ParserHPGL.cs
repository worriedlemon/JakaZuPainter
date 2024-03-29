﻿using System.Globalization;

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
            return $"{Code}[" + String.Join(",", Arguments.Select(p => p.ToString()).ToArray()) + "]";
        }
    }

    public enum CodeHPGL
    {
        IN, // Initialize, start a plotting job
        PC, // Pen color (r,g,b)
        PW, // Pen width (w)
        PU, // Pen up and move to (x, y)
        PD, // Pen down and move to (x, y)
    }

    public class ParserHPGL
    {
        private readonly char _delimiter = ';';
        private string _filePath;

        public ParserHPGL(string filePath = @"..\..\..\Resources\strokes.plt")
        {
            _filePath = filePath;
        }

        public IEnumerable<CommandHPGL> GetNextCommand()
        {
            using StreamReader reader = new(_filePath);
            string command = "";

            while (!reader.EndOfStream)
            {
                int nextChar = reader.Read();
                char c = (char)nextChar;

                if (c == _delimiter || reader.EndOfStream)
                {
                    yield return ParseCommand(command);
                    command = "";
                }
                else
                {
                    command += c;
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
