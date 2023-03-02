using System.Net;
using System.Net.Sockets;
using System.Text;
using JakaAPI.Types;

namespace JakaAPI
{
    public class JakaRobot
    {
        private readonly Socket  _socketSending;

        private readonly Socket _socketListening;

        public JakaRobot(string domain, int portSending = 10001, int portListening = 10000)
        {
            _socketSending = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketSending.Connect(new IPEndPoint(IPAddress.Parse(domain), portSending));

            _socketListening = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketListening.Bind(new IPEndPoint(IPAddress.Parse(domain), portListening));
            _socketListening.Listen(1);
        }

        public void PowerOn()
        {
            byte[] command = JakaCommand.BuildAsByteArray("power_on");
            _socketSending.Send(command);
        }

        public void PowerOff()
        {
            byte[] command = JakaCommand.BuildAsByteArray("power_off");
            _socketSending.Send(command);
        }

        public void EnableRobot()
        {
            byte[] command = JakaCommand.BuildAsByteArray("enable_robot");
            _socketSending.Send(command);
        }

        public void DisableRobot()
        {
            byte[] command = JakaCommand.BuildAsByteArray("disable_robot");
            _socketSending.Send(command);
        }

        public void JointMove(JointPositions jointPositions, double speed, double acceleration, MovementType movementType)
        {
            byte[] command = JakaCommand.BuildAsByteArray("joint_move", 
                new CommandParameter("jointPosition", $"{jointPositions}"),
                new CommandParameter("speed", $"{speed}"),
                new CommandParameter("accel", $"{acceleration}"),
                new CommandParameter("relFlag", $"{Convert.ToInt32(movementType == MovementType.Relative)}"));
            _socketSending.Send(command);
        }

        // Inverse Kinematic solution
        public void MoveInverse(CartesianPosition cartesianPosition, double speed, double acceleration)
        {
            byte[] command = JakaCommand.BuildAsByteArray("end_move",
                new CommandParameter("jointPosition", $"{cartesianPosition}"),
                new CommandParameter("speed", $"{speed}"),
                new CommandParameter("accel", $"{acceleration}"));
            _socketSending.Send(command);
        }

        // Linear Kinematic solution
        public void MoveLinear(CartesianPosition cartesianPosition, double speed, double acceleration, MovementType movementType)
        {
            byte[] command = JakaCommand.BuildAsByteArray("moveL",
                new CommandParameter("jointPosition", $"{cartesianPosition}"),
                new CommandParameter("speed", $"{speed}"),
                new CommandParameter("accel", $"{acceleration}"),
                new CommandParameter("relFlag", $"{Convert.ToInt32(movementType == MovementType.Relative)}"));
            _socketSending.Send(command);
        }

        public SendingResponse GetSendingResponce()
        {
            byte[] responseBytes = new byte[1024];
            int numBytesReceived = _socketSending.Receive(responseBytes);
            string rawJson = Encoding.ASCII.GetString(responseBytes, 0, numBytesReceived);
            return new SendingResponse(rawJson);
        }

        public string GetListeningResponce()
        {
            byte[] extraBytes = new byte[1024];
            Socket extraConnection = _socketListening.Accept();
            int numExtraBytesReceived = extraConnection.Receive(new ArraySegment<byte>(), SocketFlags.None);

            return Encoding.ASCII.GetString(extraBytes, 0, numExtraBytesReceived);
        }
    }
}
