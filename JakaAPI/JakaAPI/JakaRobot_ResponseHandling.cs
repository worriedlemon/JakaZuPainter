using System.Text;

namespace JakaAPI
{
    public partial class JakaRobot
    {
        public delegate void DebugInformation(string message);
        private event DebugInformation? FunctionFeedback;

        private readonly int _commandDelay = 100;

        private string _lastSendingResponse = string.Empty;

        // Currently unused
        private string _lastListeningResponse = string.Empty;

        public void DebugSubscribe(DebugInformation debug)
        {
            FunctionFeedback += debug;
        }

        public void DebugUnSubscribe(DebugInformation debug)
        {
            FunctionFeedback -= debug;
        }

        protected void OnPostCommand()
        {
            /*_lastSendingResponse = ReadSendingResponse();
            WaitComplete();
            _lastSendingResponse = ReadSendingResponse();
            ReadSendingResponse();
            FunctionFeedback?.Invoke(_lastSendingResponse);*/
            Thread.Sleep(_commandDelay);
        }

        private string ReadSendingResponse()
        {
            byte[] responseBytes = new byte[2048];
            int numBytesReceived = _socketSending.Receive(responseBytes);
            return Encoding.ASCII.GetString(responseBytes, 0, numBytesReceived);
        }

        public string ReadListeningResponse()
        {
            byte[] responseBytes = new byte[2048];
            int numBytesReceived = _socketListening.Receive(responseBytes);
            return Encoding.ASCII.GetString(responseBytes, 0, numBytesReceived);
        }

        public string GetLastSendingResponse() => _lastSendingResponse;

        public string GetLastListeningResponse() => _lastListeningResponse;
    }
}
