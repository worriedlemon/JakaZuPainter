using System.Net;
using System.Net.Sockets;
using System.Text;
using JakaAPI.Types;
using System.Globalization;

namespace JakaAPI
{
    /// <summary>
    /// Jaka Robot class with TCP-commands implemented as methods
    /// </summary>
    public partial class JakaRobot
    {
        private readonly Socket _socketSending;

        private readonly Socket _socketListening;

        private readonly int _commandDelay = 100;

        private string _lastSendingResponse = string.Empty;

        // Currently unused
        private string _lastListeningResponse = string.Empty;

        private bool _debugMode = true;

        /// <summary>
        /// Constructor for Jaka Robot instance
        /// </summary>
        /// <param name="domain">String representing a domain or an IP-address of a robot on LAN</param>
        /// <param name="portSending">Port for sending commands and receiving feedback</param>
        /// <param name="portListening">Port for getting robot state</param>
        public JakaRobot(string domain, int portSending = 10001, int portListening = 10000)
        {
            _socketSending = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketSending.Connect(new IPEndPoint(IPAddress.Parse(domain), portSending));

            _socketListening = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketListening.Connect(new IPEndPoint(IPAddress.Parse(domain), portListening));
            
            if (_debugMode) FunctionFeedback += Console.WriteLine;
        }

        /// <summary>
        /// Function to power the robot on
        /// </summary>
        public void PowerOn()
        {
            byte[] command = JakaCommand.BuildAsByteArray("power_on");
            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Function to power the robot off
        /// </summary>
        public void PowerOff()
        {
            byte[] command = JakaCommand.BuildAsByteArray("power_off");
            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Function for enabling robot (setting into ready mode)
        /// </summary>
        public void EnableRobot()
        {
            byte[] command = JakaCommand.BuildAsByteArray("enable_robot");
            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Function fro disabling robot (setting into wait mode)
        /// </summary>
        public void DisableRobot()
        {
            byte[] command = JakaCommand.BuildAsByteArray("disable_robot");
            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Function for getting main robot data (powered, enabled, joint positions, etc.)
        /// </summary>
        public RobotData GetRobotData()
        {
            byte[] command = JakaCommand.BuildAsByteArray("get_data");
            _socketSending.Send(command);
            return new RobotData(ReadSendingResponse());
        }

        /// <summary>
        /// Function for moving each joint
        /// </summary>
        /// <param name="jointPositions">Specific joint positions in degrees</param>
        /// <param name="speed">Sets the speed in degrees per second</param>
        /// <param name="acceleration">Sets the acceleration in degrees per second squared</param>
        /// <param name="movementType">Type of a movement (absolute or relative)</param>
        public void JointMove(JointsPosition jointPositions, double speed, double acceleration, MovementType movementType)
        {
            byte[] command = JakaCommand.BuildAsByteArray("joint_move", 
                new CommandParameter("jointPosition", $"{jointPositions}"),
                new CommandParameter("speed", speed.ToString(CultureInfo.InvariantCulture)),
                new CommandParameter("accel", acceleration.ToString(CultureInfo.InvariantCulture)),
                new CommandParameter("relFlag", $"{(int)movementType}"));

            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Function performs the inverse solution to the target point in cartesian space
        /// </summary>
        /// <param name="cartesianPosition">Specific point in cartesian space</param>
        /// <param name="speed">Sets the speed in degrees per second</param>
        /// <param name="acceleration">Sets the acceleration in degrees per second squared</param>
        public void JointInverseSolution(CartesianPosition cartesianPosition, double speed, double acceleration)
        {
            byte[] command = JakaCommand.BuildAsByteArray("end_move",
                new CommandParameter("endPosition", $"{cartesianPosition}"),
                new CommandParameter("speed", speed.ToString(CultureInfo.InvariantCulture)),
                new CommandParameter("accel", acceleration.ToString(CultureInfo.InvariantCulture)));
            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Function performs movement from the current position to the target position point in a straight line
        /// </summary>
        /// <param name="cartesianPosition">Specific point in cartesian space</param>
        /// <param name="speed">Sets the speed in degrees per second</param>
        /// <param name="acceleration">Sets the acceleration in degrees per second squared</param>
        /// <param name="movementType">Type of a movement (absolute or relative)</param>
        public void MoveLinear(CartesianPosition cartesianPosition, double speed, double acceleration, MovementType movementType)
        {
            byte[] command = JakaCommand.BuildAsByteArray("moveL",
                new CommandParameter("cartPosition", $"{cartesianPosition}"),
                new CommandParameter("speed", speed.ToString(CultureInfo.InvariantCulture)),
                new CommandParameter("accel", acceleration.ToString(CultureInfo.InvariantCulture)),
                new CommandParameter("relFlag", $"{(int)movementType}"));
            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Function for getting digital input status
        /// </summary>
        public void GetDIStatus()
        {
            byte[] command = JakaCommand.BuildAsByteArray("get_digital_input_status");
            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Function for setting digital output
        /// </summary>
        /// <param name="type">Refers to DO type: 0 - controller DO, 1 - tool DO, 2 - extend DO</param>
        /// <param name="index">Refers to DO index: 0-48</param>
        /// <param name="value">Refers to DO state: true/false for on/off</param>
        public void SetDOState(byte type, byte index, bool value)
        {
            byte[] command = JakaCommand.BuildAsByteArray("set_digital_output",
                new CommandParameter("type", type.ToString()),
                new CommandParameter("index", index.ToString()),
                new CommandParameter("value", Convert.ToInt32(value).ToString())
                );
            _socketSending.Send(command);
            OnPostCommand();
        }
    }
}
