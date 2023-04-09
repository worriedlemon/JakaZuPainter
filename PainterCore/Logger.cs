namespace PainterCore
{
    public class Logger
    {
        private readonly string _path = @"..\..\..\Log";
        private string _filePath; 

        public Logger()
        {
            Directory.CreateDirectory(_path);

            _filePath = Path.Combine(_path, CreateUniqueFileName());

            LogMessage("Started logger\n\nRobot painter commands log file\n\nDate: {DateTime.Now.Date:dd/MM/yyyy}\n");
        }

        private string CreateUniqueFileName()
        {
            DateTime currentDate = DateTime.Now.Date;
            string[] files = Directory.GetFiles(_path, currentDate.ToString("dd-MM-yyyy") + "-*.log");
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
            return $"{currentDate:dd-MM-yyyy}-{logFileNumber}.log";
        }

        public void LogMessage(string message) => File.AppendAllText(_filePath, $"[{DateTime.Now:HH:mm:ss}]: {message}\n");
    }
}
