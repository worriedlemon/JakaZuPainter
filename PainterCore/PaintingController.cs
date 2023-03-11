using JakaAPI.Types;
using PainterArm;

namespace PainterCore
{
    public class PaintingController
    {
        public PaintingController()
        {
            const string ip = "192.168.1.101";
            const int port = 10001;
            _painter = new(ip, port);

            _palette = new();

            _mixer = new();
        }

        private JakaPainter _painter;
        private Palette _palette;
        private RobotMixerDummy _mixer;

        private ColorRGB _previousColor = new ColorRGB(0, 0, 0);


        public void Start()
        {
            InitPainter();
            _painter.CalibrateSurface();
            _painter.CalibrateBrushes();
            _painter.CalibrateWasher();
            _painter.CalibrateDryer();
            _palette.CalibratePalette();

            ParserHPGL commands = new(@"..\..\..\Resources\strokes.plt");

            foreach (CommandHPGL command in commands.GetNextCommand())
            {
                Console.WriteLine("Executing: " + command);

                switch (command.Code)
                {
                    case CodeHPGL.IN:
                        break;
                    case CodeHPGL.PC:
                        BrushColor(command.Arguments);
                        break;
                    case CodeHPGL.PW:
                        break;
                    case CodeHPGL.PU:
                        break;
                    case CodeHPGL.PD:
                        BrushMove(command.Arguments);
                        break;
                }

                Thread.Sleep(1000);
            }

            DisablePainter();

            Console.ReadKey();
        }

        private void BrushColor(double[] arguments)
        {
            ColorRGB color = new(arguments[1], arguments[2], arguments[3]);
            
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

            if(color != _previousColor)
            {
                _painter.PutAsideBrush();
                _painter.PickNewBrush();
            }

            _previousColor = color;

            _painter.DunkBrush(_palette.GetColorCoordinates(color));
        }

        private void BrushMove(double[] arguments)
        {
            double x = arguments[0];
            double y = arguments[1];
            _painter.DrawLine(x, y);
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
