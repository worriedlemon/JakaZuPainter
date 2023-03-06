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

        protected void OnPostCommand()
        {
            Thread.Sleep(_commandDelay);
            _lastSendingResponse = ReadSendingResponse();
            FunctionFeedback?.Invoke(_lastSendingResponse);
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

        // Currently unused
        public string GetListeningResponse()
        {
            byte[] responseBytes = new byte[2048];
            int numBytesReceived = _socketListening.Receive(responseBytes);
            return Encoding.ASCII.GetString(responseBytes, 0, numBytesReceived);
        }
    }
}
