using System.Net;
using System.Net.Sockets;

namespace JakaAPI
{
    public class JakaRobot
    {
        private Socket _robotSocket;

        public JakaRobot(string domain, int port = 10001)
        {
            _robotSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _robotSocket.ConnectAsync(new IPEndPoint(IPAddress.Parse(domain), port));
        }

        public void PowerOn()
        {
            byte[] command = JakaCommand.BuildAsByteArray("power_on");
            _robotSocket.Send(command);
        }

        public void PowerOff()
        {
            byte[] command = JakaCommand.BuildAsByteArray("power_off");
            _robotSocket.Send(command);
        }

        public void EnableRobot()
        {
            byte[] command = JakaCommand.BuildAsByteArray("enable_robot");
            _robotSocket.Send(command);
        }

        public void DisableRobot()
        {
            byte[] command = JakaCommand.BuildAsByteArray("disable_robot");
            _robotSocket.Send(command);
        }

        public void JointMove(JointPositions jointPositions, double speed, double acceleration, MovementType movementType)
        {
            byte[] command = JakaCommand.BuildAsByteArray("joint_move", 
                new CommandParameter("jointPosition", $"{jointPositions}"),
                new CommandParameter("speed", $"{speed}"),
                new CommandParameter("accel", $"{acceleration}"),
                new CommandParameter("relFlag", $"{Convert.ToInt32(movementType == MovementType.Relative)}"));
            _robotSocket.Send(command);
        }

        // Движение с помощью инверсной кинематики
        public void MoveInverse(CartesianPosition cartesianPosition, double speed, double acceleration)
        {
            byte[] command = JakaCommand.BuildAsByteArray("end_move",
                new CommandParameter("jointPosition", $"{cartesianPosition}"),
                new CommandParameter("speed", $"{speed}"),
                new CommandParameter("accel", $"{acceleration}"));
            _robotSocket.Send(command);
        }

        // Движение с помощью прямой кинематики
        public void MoveLinear(CartesianPosition cartesianPosition, double speed, double acceleration, MovementType movementType)
        {
            byte[] command = JakaCommand.BuildAsByteArray("moveL",
                new CommandParameter("jointPosition", $"{cartesianPosition}"),
                new CommandParameter("speed", $"{speed}"),
                new CommandParameter("accel", $"{acceleration}"),
                new CommandParameter("relFlag", $"{Convert.ToInt32(movementType == MovementType.Relative)}"));
            _robotSocket.Send(command);
        }
    }
}