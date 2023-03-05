using JakaAPI;
using JakaAPI.Types;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;

class Program
{
    static void Main(string[] args)
    {
        // Setting robot LAN configuration
        const string ip = "192.168.1.100";
        const int port = 10001;

        JakaPainter painter = new JakaPainter(ip, port);

        // Every method requires a small amount of type to be executed before sending next command
        //painter.PowerOn();

        //painter.EnableRobot();

        //RobotData data = painter.GetRobotData();

        painter.JointMove(new JointsPosition(30, 0, 0, 0, 0, 0), 3, 2.5, MovementType.Relative);

        //painter.DisableRobot();

        //painter.PowerOff();

        // Waiting for input to exit a program
        Console.ReadKey();
    }
}
