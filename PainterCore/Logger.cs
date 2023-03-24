namespace PainterCore
{
    public class Logger
    {
        private readonly StreamWriter _writer;

        private readonly string _path = @"..\..\..\Log";

        public Logger()
        {
            Directory.CreateDirectory(_path);

            _writer = new StreamWriter(Path.Combine(_path, CreateUniqueFileName()));
        }

        private string CreateUniqueFileName()
        {
            DateTime currentDate = DateTime.Now.Date;

            string[] files = Directory.GetFiles(_path, currentDate.ToString("dd-MM-yyyy-*.log"));

            int logFileNumber = 1;

            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    int fileNumber = int.Parse(fileName.Substring(11)); // 11 - length of "dd-MM-yyyy-" name base
                    logFileNumber = Math.Max(logFileNumber, fileNumber);
                }
                logFileNumber++;
            }
            return $"{currentDate.ToString("dd-MM-yyyy")}-{logFileNumber}.log";
        }

        public void LogMessage(string message)
        {
            _writer.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]: {message}");
        }

        public void Close()
        {
            _writer.Close();
        }
    }
}
