using JakaAPI;

class Program
{
    static void Main(string[] args)
    {
        const string ip = "192.168.1.100";
        const int port = 10001;

        JakaRobot robot = new(ip, port);

        string JsonCommand = JakaCommand.Build("move", new CommandParameter("speed", "30"), new CommandParameter("accel", "10"));

        Console.WriteLine("Command example:\n" + JsonCommand);

        robot.PowerOn();

        robot.Enable();

        Console.WriteLine("Press [Enter] to exit...");
        Console.ReadLine();
    }
}