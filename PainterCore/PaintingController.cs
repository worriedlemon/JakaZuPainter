using PainterArm;

namespace PainterCore
{
    public class PaintingController
    {
        public PaintingController()
        {
            const string ip = "192.168.1.100";

            _painter = new(ip);
            _palette = new(_painter);
            _mixer = new();
        }

        private readonly JakaPainter _painter;
        private readonly Palette _palette;
        private readonly RobotMixerDummy _mixer;

        private ColorRGB _currentColor = new(0, 0, 0);

        const string _canvasConfigPath = @"..\..\..\Configuration\canvas_calibration.json";

        public void Start()
        {
            InitPainter();

            while (true)
            {
                Console.WriteLine("Load previous canvas calibration configuration? [Y/N]");
                Console.Write("> ");
                string input = Console.ReadLine();

                if (!(input == "Y" || input == "N"))
                {
                    Console.WriteLine("Unknown response. Try again.");
                    Console.Write("> ");
                }
                else
                {
                    CoordinateSystem2D cs;
                    if (input == "Y")
                    {
                        cs = Configuration.ConfigurationManager.LoadFromFile<CoordinateSystem2D>(_canvasConfigPath)!;
                    }
                    else
                    {
                        Console.WriteLine("---- [Canvas calibration] ----\n");
                        cs = _painter.CalibrationBehavior.Calibrate();
                        Configuration.ConfigurationManager.SaveToFile(cs, _canvasConfigPath);
                    }
                    _painter.SetCalibrationSurface(cs);
                    break;
                }
            }

            Console.WriteLine("---- [Brushes calibration] ----\n");
            _painter.CalibrateBrushes();

            Console.WriteLine("---- [Dryer calibration] ----\n");
            _painter.CalibrateDryer();

            Console.WriteLine("---- [Pallete calibration] ----\n");
            _palette.CalibratePalette();

            Console.WriteLine("Calibration ended. Press any key to continue...");
            Console.ReadKey();

            ParserHPGL commands = new(@"..\..\..\Resources\strokes2.plt");

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

            Console.WriteLine("\nPress any button to exit the program...");
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

            if (color != _currentColor)
            {
                if (_painter.CurrentBrush != -1)
                {
                    _painter.ReturnCurrentBrush();
                }

                _painter.MixWater();
                _painter.PickNewBrush(0);
                _painter.DryCurrentBrush();
            }

            _currentColor = color;

            _painter.DunkBrushInColor(_palette.GetColorCoordinates(color));
        }

        private void BrushMove(double[] arguments)
        {
            if (_palette.GetStrokesLeft(_currentColor) == 0)
            {
                _palette.UpdateColor(_currentColor);
                _mixer.MixColor(_palette.GetColorCoordinates(_currentColor), _currentColor);
            }

            double x = arguments[0];
            double y = arguments[1];
            _painter.DrawLine(x, y);
        }

        private void InitPainter()
        {
            _painter.PowerOn();
            _painter.EnableRobot();
            //_painter.GripOff();
        }

        private void DisablePainter()
        {
            //_painter.GripOff();
            _painter.DisableRobot();
            _painter.PowerOff();
        }
    }
}
