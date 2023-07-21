using PainterArm;
using PainterCore.Configuration;
using JakaAPI.Types;
using PainterArm.Stroke;
using JakaAPI.Types.Math;

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
            _logger = new();
        }

        private readonly JakaPainter _painter;
        private readonly Palette _palette;
        private readonly RobotMixerDummy _mixer;
        private readonly Logger _logger;

        private ColorRGB _currentColor = new(0, 0, 0);

        public void Start()
        {
            _painter.DebugSubscribe(_logger.LogMessage);

            Console.WriteLine("Calibration started.");
            CalibrateAllDevices();
            Console.WriteLine("Calibration ended. Press any key to continue...");
            Console.ReadKey();

            List<Stroke> strokes = new List<Stroke>()
            {
                new Stroke(new List<Point>()
                {
                   new Point(10, 10), new Point(10, 20),
                }).AddEnter().AddExit(),

                new Stroke(new List<Point>()
                {
                   new Point(20, 10), new Point(20, 30)
                }).AddEnter().AddExit(),

                new Stroke(new List<Point>()
                {
                   new Point(30, 10), new Point(30, 40)
                }).AddEnter().AddExit(),
            };

            try
            {
                BrushColor(new double[2] { 0, 0 });

                foreach (Stroke stroke in strokes)
                {
                    Console.WriteLine("New stroke");
                    foreach (Point point in stroke.GetPoints())
                    {
                        Console.WriteLine("Point: " + point);
                        _painter.DrawLine(point.X, point.Y, point.Z);
                        Thread.Sleep(500);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"\n---- Error ---- \n + {exception.Message}");
            }


            //DisablePainter();

            Console.WriteLine("\nPress any button to exit the program...");
            Console.ReadKey();
        }

        private void CalibrateAllDevices()
        {
            // Canvas calibration
            ConfigurationManager.CalibrationDialog(out CoordinateSystem2D canvasCoordinateSystem,
                _painter.CanvasCalibrationBehavior,
                @"..\..\..\Configuration\canvas_calibration.json",
                "Canvas calibration");
            _painter.CalibrateCanvas(canvasCoordinateSystem);
            Console.WriteLine($"Calibrated coordinates:\n{canvasCoordinateSystem}\n{canvasCoordinateSystem.RPYParameters}\n");
            _logger.LogMessage($"Calibrated coordinates:\n{canvasCoordinateSystem}\n{canvasCoordinateSystem.RPYParameters}\n");

            // Brushes calibration
            ConfigurationManager.CalibrationDialog(out LocationDictionary brushesLocations,
                _painter.BrushesCalibrationBehavior,
                @"..\..\..\Configuration\brushes_calibration.json",
                "Brushes calibration");
            _painter.CalibrateBrushes(brushesLocations);

            // Dryer calibration
            ConfigurationManager.CalibrationDialog(out LocationDictionary dryerLocations,
                _painter.DryerCalibrationBehavior,
                @"..\..\..\Configuration\dryer_calibration.json",
                "Dryer calibration");
            _painter.CalibrateDryer(dryerLocations[dryerLocations.Count - 1]);

            // Palette calibration
            ConfigurationManager.CalibrationDialog(out CoordinateSystem2D paletteCoordinateSystem,
                _palette.CalibrationBehavior,
                @"..\..\..\Configuration\palette_calibration.json",
                "Palette calibration");
            _palette.CalibratePalette(paletteCoordinateSystem);
        }

        private void BrushColor(double[] arguments)
        {
            _painter.DunkBrushInColor(_palette.GetAvaliableLocation());

            return;

            // Сам скрипт
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

        private void BrushMove(double[] arguments, double zOffset, bool pressed = true)
        {
            if (false && _palette.GetStrokesLeft(_currentColor) == 0)
            {
                _palette.UpdateColor(_currentColor);
                _mixer.MixColor(_palette.GetColorCoordinates(_currentColor), _currentColor);
            }

            _painter.DrawLine(arguments[0], arguments[1], zOffset);
        }

        private void InitPainter()
        {
            _painter.PowerOn();
            _painter.EnableRobot();
        }

        private void DisablePainter()
        {
            _painter.GripOff();
            _painter.DisableRobot();
            _painter.PowerOff();
        }
    }
}
