using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakaAPI
{
    public partial class JakaRobot
    {
        delegate void DebugInformation(string message);
        event DebugInformation? FunctionFeedback;

        private readonly int _dragStatusDelay = 333;

        protected void OnPostCommand()
        {
            Thread.Sleep(_commandDelay);
            _lastListeningResponse = ReadListeningResponse();
            FunctionFeedback?.Invoke(_lastListeningResponse);
        }

        private async Task DraggingEndAsync()
        {
            while(GetDragStatus())
            {
                await Task.Delay(_dragStatusDelay);
            }
        }

        private string ReadSendingResponse()
        {
            byte[] responseBytes = new byte[2048];
            int numBytesReceived = _socketSending.Receive(responseBytes);
            return Encoding.ASCII.GetString(responseBytes, 0, numBytesReceived);
        }

        public string GetLastSendingResponse()
        {
            return _lastSendingResponse;
        }

        public string GetLastListeningResponse()
        {
            return _lastListeningResponse;
        }

        public string ReadListeningResponse()
        {
            byte[] responseBytes = new byte[2048];
            int numBytesReceived = _socketListening.Receive(responseBytes);
            return Encoding.ASCII.GetString(responseBytes, 0, numBytesReceived);
        }
    }
}
