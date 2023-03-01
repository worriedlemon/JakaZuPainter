using JakaAPI;
using Microsoft.VisualBasic;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        const string ip = "192.168.1.102";
        const int port = 10001;

        JakaRobot robot = new(ip, port);

        string JsonCommand = JakaCommand.Build("move", new CommandParameter("speed", "30"), new CommandParameter("accel", "10"));

        Console.WriteLine(JsonCommand);

        robot.PowerOn();

        robot.Enable();

        Console.ReadLine();
    }
}