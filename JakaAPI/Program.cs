using JakaAPI;
using JakaAPI.Types;

class Program
{
    static void Main(string[] args)
    {
        // Setting robot LAN configuration
        const string ip = "192.168.1.100";
        const int port = 10001;

        JakaPainter painter = new(ip, port);

        // Every method requires a small amount of type to be executed before sending next command
        painter.PowerOn();
        Thread.Sleep(100);

        painter.EnableRobot();
        Thread.Sleep(100);

        painter.JointMove(new JointPositions(30, 0, 0, 0, 0, 0), 3, 2.5, MovementType.Relative);
        Thread.Sleep(100);

        painter.GetRobotData();
        Thread.Sleep(100);

        painter.DisableRobot();
        Thread.Sleep(100);

        painter.PowerOff();

        // Waiting for input to exit a program
        Console.ReadKey();
    }

}
