using System.Net.Sockets;

namespace JakaAPI
{
    public class JakaRobot
    {
        private TcpClient client;
        private NetworkStream ns;

        public JakaRobot(string domain, int port = 8080)
        {
            //listener = new TcpListener(System.Net.IPAddress.Parse(domain), port);
            client = new TcpClient(domain, port);
            ns = client.GetStream();
            WaitForResponse(1024);
        }

        private async void WaitForResponse(int bufferSize)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (ns.DataAvailable)
                    {
                        byte[] buffer = new byte[bufferSize];
                        ns.Read(buffer);
                        Console.WriteLine(buffer);
                    }
                }
            });
        }

        public void PowerOn()
        {
            byte[] command = JakaCommand.BuildAsByteArray("power_on");
            ns.Write(command);
        }

        public void PowerOff()
        {
            byte[] command = JakaCommand.BuildAsByteArray("power_off");
            ns.Write(command);
        }

        public void Enable()
        {
            byte[] command = JakaCommand.BuildAsByteArray("enable_robot");
            ns.Write(command);
        }
    }
}
