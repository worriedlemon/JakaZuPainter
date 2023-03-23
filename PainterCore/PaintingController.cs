using JakaAPI.Types.Math;
using PainterArm;

namespace PainterCore
{
    public class PaintingController
    {
        public PaintingController()
        {
            const string ip = "192.168.1.101";

            _painter = new(ip);
            _palette = new(_painter);
            _mixer = new();
        }

        private readonly JakaPainter _painter;
        private readonly Palette _palette;
        private readonly RobotMixerDummy _mixer;

        private ColorRGB _currentColor = new(0, 0, 0);

        public void Start()
        {
            InitPainter();

            CalibrationDialog(ref _painter.GetCanvasCoordinateSystemReference(), _painter.CanvasCalibrationBehavior.Calibrate, @"..\..\..\Configuration\canvas_calibration.json", "Canvas calibration");

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
                        _painter.BrushOrthogonal(100);
                        BrushMove(command.Arguments);
                        break;
                    case CodeHPGL.PD:
                        _painter.BrushOrthogonal(0);
                        BrushMove(command.Arguments);
                        break;
                }

                Thread.Sleep(1000);
            }

            DisablePainter();

            Console.WriteLine("\nPress any button to exit the program...");
            Console.ReadKey();
        }

        /// <summary>
        /// Experimental function on loading and saving calibration settings of different devices
        /// </summary>
        /// <typeparam name="T">Data structure of device calibration configuration</typeparam>
        /// <param name="loadableObject">Object, where the calibration is being used</param>
        /// <param name="actionOnCalibrate">Function, which is invoked for calibration</param>
        /// <param name="configPath">Path to save file</param>
        /// <param name="configName">Headline of current dialog</param>
        private static void CalibrationDialog<T>(ref T loadableObject, Func<T> actionOnCalibrate, string configPath, string configName = "Calibration")
        {
            while (true)
            {
                Console.WriteLine($"---- [{configName}] ----\n");
                Console.WriteLine("Load previous configuration? [Y/N]");
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
                        loadableObject = Configuration.ConfigurationManager.LoadFromFile<T>(configPath)!;
                    }
                    else
                    {
                        loadableObject = actionOnCalibrate.Invoke();
                        Configuration.ConfigurationManager.SaveToFile(loadableObject, configPath);
                    }
                    break;
                }
            }
            Console.WriteLine();
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

            _painter.DrawLine(arguments[0], arguments[1]);
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
