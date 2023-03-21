using JakaAPI.Types;
using JakaAPI.Types.Math;
using JakaAPI;

namespace PainterArm
{
    /// <summary>
    /// Jaka Robot based implementation of painting robot
    /// </summary>
    public class JakaPainter : JakaRobot
    {
        private CoordinateSystem2D? _canvasCoordinateSystem;

        private Dictionary<int, CartesianPosition> _brushesLocations;
        private CartesianPosition _washerLocation;
        private CartesianPosition _dryerLocation;

        private int _currentBrush = -1;

        /// <summary>
        /// Indicates whether the grip of the robot is being in grap state
        /// </summary>
        private bool _grip;

        public JakaPainter(string domain, int portSending = 10001, int portListening = 10000)
            : base(domain, portSending, portListening)
        {
            _brushesLocations = new Dictionary<int, CartesianPosition>();
            _grip = false;
        }

        /// <summary>
        /// Manual canvas calibration by three points
        /// </summary>
        public CoordinateSystem2D CalibrateSurface()
        {
            byte complete = 0;
            Point zero = new(), axisX = new(), axisY = new();
            RPYMatrix canvasRPY = new(180, 0, 0);
            
            Console.WriteLine("---- [Surface calibration] ----\n" +
                "(1) Set zero pivot point\n" +
                "(2) Set X-axis point\n" +
                "(3) Set Y-axis point\n" +
                "(0) End calibration");

            while (true)
            {
                Console.Write("> ");
                int option = Int32.Parse(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        zero = GetRobotData().ArmCartesianPosition.Point;
                        canvasRPY = GetRobotData().ArmCartesianPosition.Rpymatrix;
                        complete |= 1;
                        break;
                    case 2:
                        axisX = GetRobotData().ArmCartesianPosition.Point;
                        complete |= 2;
                        break;
                    case 3:
                        axisY = GetRobotData().ArmCartesianPosition.Point;
                        complete |= 4;
                        break;
                    case 0:
                        if (complete != 7)
                        {
                            Console.WriteLine("Calibration is not complete. Please, set missing points!");
                            break;
                        }

                        _canvasCoordinateSystem = new(zero, axisX, axisY, canvasRPY);

                        Console.WriteLine($"Calibrated coordinates:\n{_canvasCoordinateSystem}");

                        return _canvasCoordinateSystem;
                    default:
                        Console.WriteLine("Unknown option. Try again.");
                        break;
                }
            }
        }

        /// <summary>
        /// Canvas calibration based on existing <see cref="CoordinateSystem2D"/>
        /// </summary>
        /// <param name="cs">Existing coordinate system to be used as canvas</param>
        public void CalibrateSurface(CoordinateSystem2D cs)
        {
            _canvasCoordinateSystem = cs;
            Console.WriteLine($"Calibrated coordinates:\n{_canvasCoordinateSystem}");
        }

        /// <summary>
        /// Brushes calibration by a point
        /// </summary>
        public void CalibrateBrushes()
        {
            Console.WriteLine("---- [Brushes calibration] ----\n" +
                "(1) Add new brush location\n" +
                "(0) End calibration");

            while (true)
            {
                int option = Int32.Parse(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        _brushesLocations.Add(_brushesLocations.Count, GetRobotData().ArmCartesianPosition);
                        break;
                    case 2:
                        return;
                }
            }
        }

        /// <summary>
        /// Washer calibration by a point
        /// </summary>
        public void CalibrateWasher()
        {
            Console.WriteLine("---- [Washer calibration] ----\n" +
                "(1)Add washer location\n" +
                "(0)End calibration");
            while (true)
            {
                int option = Int32.Parse(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        _washerLocation = GetRobotData().ArmCartesianPosition;
                        break;
                    case 0:
                        return;
                }
            }

        }

        /// <summary>
        /// Dryer calibration by a point
        /// </summary>
        public void CalibrateDryer()
        {
            Console.WriteLine("---- [Dryer calibration] ----\n" +
                "(1) Add dryer location\n" +
                "(0) End calibration");
            while (true)
            {
                int option = Int32.Parse(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        _dryerLocation = GetRobotData().ArmCartesianPosition;
                        break;
                    case 2:
                        return;
                }
            }
        }
        
        /// <summary>
        /// Draw line with canvas 2D coordinates
        /// </summary>
        /// <param name="x">X-axis offset in millimeters <i>(or special units like 25 micron?)</i></param>
        /// <param name="y">Y-axis offset in millimeters</param>
        public void DrawLine(double x, double y)
        {
            Point point3d = _canvasCoordinateSystem!.CanvasPointToWorldPoint(x, y);
            Console.WriteLine(point3d);

            MoveLinear(new CartesianPosition(point3d, _canvasCoordinateSystem.CanvasRPY), 10, 5, MovementType.Absolute);
        }

        // Put current brush to [washer -> dryer -> active zone] cycle
        public void PutAsideBrush()
        {
            //UpdateBrushesState();
        }

        // Pick new clear brush
        public void PickNewBrush()
        {
            CartesianPosition brushPosition = _brushesLocations[0];
            Point brushPoint = brushPosition.Point;
            Point upperPoint = new Point(brushPoint.X, brushPoint.Y, brushPoint.Z + 30);
            RPYMatrix orthogonalRPY = brushPosition.Rpymatrix;

            // Move to position above the brush
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

            GripOff();

            // Move to brush on stand
            MoveLinear(new CartesianPosition(brushPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

            GripOn();

            // Move to position above the palete again
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 100, 25, MovementType.Absolute);
            //UpdateBrushesState();
        }

        // Dunk current brush it the palette color
        public void DunkBrush(CartesianPosition colorPosition)
        {
            Point colorPoint = colorPosition.Point;
            Point upperPoint = new Point(colorPoint.X, colorPoint.Y, colorPoint.Z + 30);
            // Grip position, orthogonal to the palette surface
            RPYMatrix orthogonalRPY = new RPYMatrix(-180, 0, 0);

            // Move to position above the palete
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 10, 5, MovementType.Absolute);

            // Move to color on palette
            MoveLinear(new CartesianPosition(colorPoint, orthogonalRPY), 10, 5, MovementType.Absolute);

            // Move to position above the palete again
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 10, 5, MovementType.Absolute);
        }

        // Grip methods
        public void GripOn()
        {
            _grip = true;
            SetDOState(0, 0, _grip);
        }

        public void GripOff()
        {
            _grip = false;
            SetDOState(0, 0, _grip);
        }

        public void ToggleGrip()
        {
            _grip = !_grip;
            SetDOState(0, 0, _grip);
        }
    }
}
