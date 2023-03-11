using System.Net;
using System.Net.Sockets;
using JakaAPI.Types;
using System.Globalization;
using System.Text.Json.Nodes;

namespace JakaAPI
{
    /// <summary>
    /// Jaka Robot class with TCP-commands implemented as methods
    /// </summary>
    public partial class JakaRobot
    {
        private readonly Socket _socketSending;

        private readonly Socket _socketListening;

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

        #region Powering and connection commands

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
        /// Function that shuts the robot and the controller down
        /// </summary>
        public void Shutdown()
        {
            byte[] command = JakaCommand.BuildAsByteArray("shutdown");
            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Function that quits the current connection
        /// </summary>
        public void Disconnect()
        {
            byte[] command = JakaCommand.BuildAsByteArray("quit");
            _socketSending.Send(command);
            OnPostCommand();
        }

        #endregion

        #region Movement commands

        /// <summary>
        /// Completely stops the robot movement caused by linear/kinematic/program moves
        /// </summary>
        public void StopMovement()
        {
            byte[] command = JakaCommand.BuildAsByteArray("stop_program");
            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Sets the rapid rate of robot (simply the overall velocity)
        /// </summary>
        /// <param name="rateValue">Rapid rate value: 0.0-1.0</param>
        public void SetRapidRate(double rateValue)
        {
            byte[] command = JakaCommand.BuildAsByteArray("rapid_rate",
                new CommandParameter("rate_value", rateValue.ToString(CultureInfo.InvariantCulture)));
            _socketSending.Send(command);
            OnPostCommand();
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

        #endregion

        #region IO commands

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
        /// Function for triggering digital output
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

        /// <summary>
        /// Function for triggering analog output
        /// </summary>
        /// <param name="type">Refers to AO type: 0 - controller AO, 1 - tool AO, 2 - extend AO</param>
        /// <param name="index">Refers to AO index: 0-7</param>
        /// <param name="value">Refers to AO state: true/false for on/off</param>
        public void SetAOState(byte type, byte index, bool value)
        {
            byte[] command = JakaCommand.BuildAsByteArray("set_analog_output",
                new CommandParameter("type", type.ToString()),
                new CommandParameter("index", index.ToString()),
                new CommandParameter("value", Convert.ToInt32(value).ToString())
                );
            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Getting information about function input pins
        /// </summary>
        public void GetFunctionDIStatus()
        {
            byte[] command = JakaCommand.BuildAsByteArray("get_funcdi_status");
            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Getting information about external extension IO module
        /// </summary>
        public void GetExtensionIOStatus()
        {
            byte[] command = JakaCommand.BuildAsByteArray("get_extio_status");
            _socketSending.Send(command);
            OnPostCommand();
        }

        #endregion

        #region Status commands

        /// <summary>
        /// Function for getting main robot data (powered, enabled, joint positions, etc.)
        /// </summary>
        public RobotData GetRobotData()
        {
            byte[] command = JakaCommand.BuildAsByteArray("get_data");
            _socketSending.Send(command);
            return new RobotData(ReadListeningResponse());
        }

        /// <summary>
        /// Function for getting drag status
        /// </summary>
        /// <returns>True if the robot is currently is in drag state</returns>
        public bool GetDragStatus()
        {
            byte[] command = JakaCommand.BuildAsByteArray("drag_status");
            _socketSending.Send(command);
            Thread.Sleep(_commandDelay);
            return JsonNode.Parse(ReadListeningResponse())!.AsObject()["drag_status"]!.GetValue<string>() == "True";
        }

        /// <summary>
        /// Function for finding out whether the robot is in protective stop status
        /// </summary>
        /// <returns>True if the robot is currently is in protective stop status</returns>
        public bool GetProtectiveStopStatus()
        {
            byte[] command = JakaCommand.BuildAsByteArray("protective_stop");
            _socketSending.Send(command);
            Thread.Sleep(_commandDelay);
            return JsonNode.Parse(ReadListeningResponse())!.AsObject()["protective_stop"]!.GetValue<int>() == 1;
        }

        #endregion

        #region Miscellaneous

        /// <summary>
        /// Sets robot collision level
        /// </summary>
        /// <param name="sensitivity">Describes the level of sensitivity: 0 is close collision protection, 1 to 5 is collision sensitivity from highest to lowest</param>
        public void SetRobotCollisionLevel(byte sensitivity)
        {
            byte[] command = JakaCommand.BuildAsByteArray("set_clsn_sensitivity",
                new CommandParameter("sensitivityVal", sensitivity.ToString()));
            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Gets the current collision level
        /// </summary>
        /// <returns>Current robot collision level on scale from 0 to 5 (highest to lowest sensitivity)</returns>
        public byte GetRobotCollisionLevel()
        {
            byte[] command = JakaCommand.BuildAsByteArray("get_clsn_sensitivity");
            _socketSending.Send(command);
            Thread.Sleep(_commandDelay);
            return JsonNode.Parse(ReadListeningResponse())!.AsObject()["sensitivityVal"]!.GetValue<byte>();
        }

        /// <summary>
        /// Sets the payload to the robot
        /// </summary>
        /// <param name="kg">Mass of payload</param>
        /// <param name="centroid">Centroid of payload</param>
        public void SetPayload(double kg, Point centroid)
        {
            byte[] command = JakaCommand.BuildAsByteArray("set_payload",
                new CommandParameter("mass", kg.ToString(CultureInfo.InvariantCulture)),
                new CommandParameter("centroid", centroid.ToString()));
            _socketSending.Send(command);
            OnPostCommand();
        }

        /// <summary>
        /// Gets the information about payload of the robot
        /// </summary>
        public void GetPayload()
        {
            byte[] command = JakaCommand.BuildAsByteArray("get_payload");
            _socketSending.Send(command);
            OnPostCommand();
        }

        [Obsolete("Command is not recommended")]
        public void WaitComplete()
        {
            byte[] command = JakaCommand.BuildAsByteArray("wait_complete");
            _socketSending.Send(command);
            OnPostCommand();
        }

        #endregion
    }
}
