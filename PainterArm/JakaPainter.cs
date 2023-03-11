using JakaAPI.Types;
using JakaAPI;

namespace PainterArm
{
    /// <summary>
    /// Jaka Robot based implementation of painting robot
    /// </summary>
    public class JakaPainter : JakaRobot
    {
        private CoordinateSystem2D _canvasCoordinateSystem = new();

        private bool _isCalibrated = false;

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
            SetDOState(0, 0, _grip);
        }

        // Calibtration
        public void CalibrateSurface()
        {
            Console.WriteLine("---Surface calibration----\n" +
                "1)Add LeftBottom point\n" +
                "2)Add LeftTop point\n" +
                "3)Add RightBottom point\n" +
                "4)End calibration");
            while (true)
            {
                int option = Int32.Parse(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        _canvasCoordinateSystem.Zero = GetRobotData().ArmCartesianPosition.Point;
                        _canvasCoordinateSystem.CanvasRPY = GetRobotData().ArmCartesianPosition.Rpymatrix;
                        break;
                    case 2:
                        _canvasCoordinateSystem.AxisY = GetRobotData().ArmCartesianPosition.Point;
                        break;
                    case 3:
                        _canvasCoordinateSystem.AxisX = GetRobotData().ArmCartesianPosition.Point;
                        break;
                    case 4:
                        return;
                }
            }
            /*switch (calibrationPoint)
            {
                case CalibrationPoint.LeftBottom:
                    _canvasCoordinateSystem.Zero = GetRobotData().ArmCartesianPosition.Point;
                    break;
                case CalibrationPoint.LeftTop:
                    _canvasCoordinateSystem.AxisY = GetRobotData().ArmCartesianPosition.Point;
                    break;
                case CalibrationPoint.RightBottom:
                    _canvasCoordinateSystem.AxisX = GetRobotData().ArmCartesianPosition.Point;
                    break;
            }*/
        }

        public void CalibrateBrushes()
        {
            Console.WriteLine("---Brushes calibration----\n1)Add new brush location\n2)End calibration");
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

        public void CalibrateWasher()
        {
            Console.WriteLine("---Washer calibration----\n1)Add washer location\n2)End calibration");
            while (true)
            {
                int option = Int32.Parse(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        _washerLocation = GetRobotData().ArmCartesianPosition;
                        break;
                    case 2:
                        return;
                }
            }

        }

        public void CalibrateDryer()
        {
            Console.WriteLine("---Dryer calibration----\n1)Add dryer location\n2)End calibration");
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
        
        // Draw line with canvas 2D coordinates
        public void DrawLine(double x, double y)
        {
            Point point3d = _canvasCoordinateSystem.Point2DToRealPoint(x, y);
            RPYMatrix canvasRPY = _canvasCoordinateSystem.CanvasRPY;

            MoveLinear(new CartesianPosition(point3d, canvasRPY), 10, 5, MovementType.Absolute);
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
            Point brushPoiunt = brushPosition.Point;
            Point upperPoint = new Point(brushPoiunt.X, brushPoiunt.Y, brushPoiunt.Z + 30);
            RPYMatrix orthogonalRPY = new RPYMatrix(-180, 0, 0);

            // Move to position above the brush
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 10, 5, MovementType.Absolute);

            GripOff();

            // Move to brush on stand
            MoveLinear(new CartesianPosition(brushPoiunt, orthogonalRPY), 10, 5, MovementType.Absolute);

            GripOn();

            // Move to position above the palete again
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 10, 5, MovementType.Absolute);
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
