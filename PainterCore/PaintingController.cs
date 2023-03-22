using JakaAPI.Types;
using PainterArm;
using System.Drawing;
using System.Reflection;

namespace PainterCore
{
    public class PaintingController
    {
        public PaintingController()
        {
            const string ip = "192.168.1.100";
            const int port = 10001;
            _painter = new(ip, port);

            _palette = new();

            _mixer = new();
        }

        private JakaPainter _painter;
        private Palette _palette;
        private RobotMixerDummy _mixer;

        private ColorRGB _currentColor = new ColorRGB(0, 0, 0);

        const string _configPath = @"..\..\..\Configuration\calibration.json";

        public void Start()
        {
            InitPainter();

            while (true)
            {
                Console.WriteLine("Load previous calibration configuration? [Y/N]");
                Console.Write("> ");
                string input = Console.ReadLine();

                if (!(input == "Y" || input == "N"))
                {
                    Console.WriteLine("Unknown response. Try again.");
                    Console.Write("> ");
                }
                else
                {
                    if (input == "Y")
                    {
                        CoordinateSystem2D loaded = Configuration.ConfigurationManager.LoadFromFile<CoordinateSystem2D>(_configPath)!;
                        _painter.CalibrateSurface(loaded);
                    }
                    else if (input == "N")
                    {
                        CoordinateSystem2D saved = _painter.CalibrateSurface();
                        Configuration.ConfigurationManager.SaveToFile(saved, _configPath);
                    }
                    break;
                }
            }

            //_painter.CalibrateBrushes();
            //_painter.CalibrateDryer();
            //_palette.CalibratePalette();

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
            _painter.GripOff();
        }

        private void DisablePainter()
        {
            _painter.GripOff();
            _painter.DisableRobot();
            _painter.PowerOff();
        }
    }
}
