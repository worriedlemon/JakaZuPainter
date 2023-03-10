using JakaAPI.Types;
using PainterArm;
using System.Reflection;

namespace PainterCore
{
    public class PaintingController
    {
        public PaintingController()
        {
            /*const string ip = "192.168.1.101";
            const int port = 10001;
            _painter = new(ip, port);

            _palette = new();

            _mixer = new();*/
        }

        private JakaPainter _painter;
        private Palette _palette;
        private RobotMixerDummy _mixer;

        public void Start()
        {
            /* InitPainter();
            _painter.StartCalibration();

            _palette.CalibratePalette();*/

            ParserHPGL commands = new(@"..\..\..\Resources\strokes.plt");

            foreach (CommandHPGL command in commands.GetNextCommand())
            {
                Console.WriteLine("Executing: " + command);

                switch (command.Code)
                {
                    case CodeHPGL.IN:
                        break;
                    case CodeHPGL.PC:
                        BrushColor(command);
                        break;
                    case CodeHPGL.PW:
                        break;
                    case CodeHPGL.PU:
                    case CodeHPGL.PD:
                        BrushMove(command);
                        break;
                }

                Task.Delay(1000);
            }

            DisablePainter();

            Console.ReadKey();
        }


        // Take free brush and paint it with color
        private void BrushColor(CommandHPGL command)
        {
            ColorRGB color = new(command.Arguments[1], command.Arguments[2], command.Arguments[3]);
            
            if (_palette.IsColorAdded(color))
            {
                if (_palette.GetStrokesLeft(color) == 0)
                {
                    _palette.UpdateColor(color);
                    _mixer.MixColor(_palette.GetColorCoordinates(color), color);
                }
            }
            else
            {
                _palette.AddNewColor(color);
                _mixer.MixColor(_palette.GetColorCoordinates(color), color);
            }

            _palette.SubstractStroke(color);
            //_painter.BrushTakeAvaliable();
            _painter.BrushColor(_palette.GetColorCoordinates(color));
        }

        private void BrushMove(CommandHPGL command)
        {
            // Add 2D -> 3D function
            // _painter.BrushDrawLine();
        }

        private void InitPainter()
        {
            _painter.PowerOn();
            _painter.EnableRobot();
        }

        private void DisablePainter()
        {
            _painter.DisableRobot();
            _painter.PowerOff();
        }

    }
}
