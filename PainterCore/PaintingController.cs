using JakaAPI.Types;
using PainterArm;
using System.Reflection;

namespace PainterCore
{
    public class PaintingController
    {
        public PaintingController() { }

        private async Task wait()
        {
            Console.WriteLine("2");
            await Task.Delay(3000);
            Console.WriteLine("3");
        }

        public async Task Start()
        {
            const string ip = "192.168.1.100";
            const int port = 10001;
            //JakaPainter painter = new(ip, port);
            //painter.StartCalibration();

            Palette palette = new();
            palette.CalibratePalette();

            ParserHPGL commands = new(@"..\..\..\Resources\strokes.plt");

            foreach (CommandHPGL command in commands.GetNextCommand())
            {
                Console.WriteLine("Executing: " + command);

                switch (command.Code)
                {
                    case CodeHPGL.IN:
                        break;
                    case CodeHPGL.PC:                        
                        break;
                    case CodeHPGL.PW:
                        break;
                    case CodeHPGL.PU:
                        break;
                    case CodeHPGL.PD:
                        break;
                }

                await Task.Delay(1000);
            }

            Console.ReadKey();

            //painter.PowerOn();

            //painter.EnableRobot();

            //RobotData data = painter.GetRobotData();

            //painter.JointMove(new JointsPosition(30, 0, 0, 0, 0, 0), 3, 2.5, MovementType.Relative);

            //painter.DisableRobot();

            //painter.PowerOff();

            // Waiting for input to exit a program

        }
    }
}
