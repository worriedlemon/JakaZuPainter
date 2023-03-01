using JakaAPI;

class Program
{
    static void Main(string[] args)
    {
        const string ip = "192.168.1.100";

        const int port = 10001;


        Console.WriteLine(Convert.ToInt32(true));


        JakaPainter painter = new JakaPainter(ip, port);

        painter.EnableRobot();


        Console.WriteLine("Press [Enter] to exit...");
        Console.ReadLine();
    }
}