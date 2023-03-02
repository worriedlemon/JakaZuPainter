using JakaAPI;
using JakaAPI.Types;

class Program
{
    static void Main(string[] args)
    {
        const string ip = "192.168.1.100";

        const int port = 10001;

        JakaPainter painter = new JakaPainter(ip, port);

        painter.PowerOn();
        Console.WriteLine(painter.GetSendingResponce().rawJson);

        painter.EnableRobot();
        Console.WriteLine(painter.GetSendingResponce().rawJson);

        Console.WriteLine("Press [Enter] to exit...");
        Console.ReadLine();
    }

}
