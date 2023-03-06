using PainterArm;

namespace PainterCore
{
    public class PaintingController
    {
        public PaintingController() { }

        public void Start()
        {
            const string ip = "192.168.1.100";
            const int port = 10001;

            ParserHPGL p = new ParserHPGL(@"..\..\..\Resources\strokes.plt");

            foreach (CommandHPGL command in p.GetNextCommand())
            {               
                Console.WriteLine("Command: " + command);

                switch(command.Code)
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

                //RgbColor color = new RgbColor();


                Thread.Sleep(1000);
            }

            //JakaPainter painter = new JakaPainter(ip, port);

            //painter.StartCalibration();

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
