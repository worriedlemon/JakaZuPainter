using JakaAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PainterCore
{
    public class PaintingController
    {
        public PaintingController() { }

        public void Start()
        {
            // Setting robot LAN configuration
            const string ip = "192.168.1.100";
            const int port = 10001;

            JakaPainter painter = new JakaPainter(ip, port);

            painter.StartCalibration();

            // Every method requires a small amount of type to be executed before sending next command
            //painter.PowerOn();

            //painter.EnableRobot();

            //RobotData data = painter.GetRobotData();


            //painter.JointMove(new JointsPosition(30, 0, 0, 0, 0, 0), 3, 2.5, MovementType.Relative);


            //painter.DisableRobot();

            //painter.PowerOff();

            // Waiting for input to exit a program
            Console.ReadKey();
        }
    }
}
