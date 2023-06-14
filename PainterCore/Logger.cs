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

            LogMessage($"Started logger\n\nRobot painter commands log file\n\nDate: {DateTime.Now.Date:d}\n");
        }

        private string CreateUniqueFileName()
        {
            DateTime currentDate = DateTime.Now.Date;
            string[] logFiles = Directory.GetFiles(_path, $"{currentDate:dd-MM-yyyy}-*.log");
            return $"{currentDate:dd-MM-yyyy}-{logFiles.Length + 1}.log";
        }

        public void LogMessage(string message) => File.AppendAllText(_filePath, $"[{DateTime.Now:t}]: {message}\n");
    }
}
