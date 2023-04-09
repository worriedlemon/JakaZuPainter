using JakaAPI.Types;
using JakaAPI.Types.Math;
using JakaAPI;
using PainterArm.Calibration;

namespace PainterArm
{
    /// <summary>
    /// Jaka Robot based implementation of painting robot
    /// </summary>
    public class JakaPainter : JakaRobot
    {
        private CoordinateSystem2D? _canvasCoordinateSystem;
        private double _currentX, _currentY, _currentHeight;

        private LocationDictionary _brushesLocations;
        private CartesianPosition _dryerLocation;
        private const int _brushLength = 100;
        public int CurrentBrush { get; private set; }

        /// <summary>
        /// Indicates whether the grip of the robot is being in grap state
        /// </summary>
        private bool _grip;

        public AbstractCalibrationBehavior CanvasCalibrationBehavior, BrushesCalibrationBehavior, DryerCalibrationBehavior;

        public JakaPainter(string domain, int portSending = 10001, int portListening = 10000)
            : base(domain, portSending, portListening)
        {
            _brushesLocations = new LocationDictionary();
            _grip = false;
            _currentX = 0;
            _currentY = 0;
            _currentHeight = 0;

            CurrentBrush = -1;
            SetDOState(0, 0, _grip);
            CanvasCalibrationBehavior = new ManualThreePointCalibration(this);
            DryerCalibrationBehavior = new ManualOnePointCalibration(this);
            BrushesCalibrationBehavior = new ManualOnePointCalibration(this);
        }

        /// <summary>
        /// Canvas calibration based on existing <see cref="CoordinateSystem2D"/>
        /// </summary>
        /// <param name="cs">Existing coordinate system to be used as canvas</param>
        public void CalibrateCanvas(CoordinateSystem2D cs) => _canvasCoordinateSystem = cs;

        /// <summary>
        /// Brushes calibration
        /// </summary>
        public void CalibrateBrushes(LocationDictionary locations) => _brushesLocations = locations;

        /// <summary>
        /// Dryer calibration
        /// </summary>
        public void CalibrateDryer(CartesianPosition location) => _dryerLocation = location;

        /// <summary>
        /// Draw line with canvas 2D coordinates
        /// </summary>
        /// <param name="x">X-axis offset in millimeters <i>(or special units like 25 micron?)</i></param>
        /// <param name="y">Y-axis offset in millimeters</param>
        public void DrawLine(double x, double y)
        {
            Point point3d = _canvasCoordinateSystem!.CanvasPointToWorldPoint(_currentX = x, _currentY = y, _currentHeight);
            MoveLinear(new CartesianPosition(point3d, _canvasCoordinateSystem.RPYParameters), 100, 25, MovementType.Absolute);
        }

        // Create water vortex, not implemented yet
        public void MixWater()
        {
            Console.WriteLine("Water vortex start...");
            Thread.Sleep(1000);
            Console.WriteLine("Water vortex end...");
        }

        /// <summary>
        /// Move the brush perpendicular to the canvas
        /// </summary>
        /// <param name="height">Z-axis offset</param>
        public void BrushOrthogonalMove(double height)
        {
            Point point3d = _canvasCoordinateSystem!.CanvasPointToWorldPoint(_currentX, _currentY, _currentHeight = height);
            MoveLinear(new CartesianPosition(point3d, _canvasCoordinateSystem.RPYParameters), 100, 25, MovementType.Absolute);
        }

        /// <summary>
        /// Returns current held brush to the stand
        /// </summary>
        public void ReturnCurrentBrush()
        {   
            CartesianPosition brushPosition = _brushesLocations[CurrentBrush];
            Point brushPoint = brushPosition.Point;
            Point upperPoint = new Point(brushPoint.X, brushPoint.Y, brushPoint.Z + _brushLength);
            RPYMatrix orthogonalRPY = brushPosition.Rpymatrix;

            // Move to position above the brush
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

            // Move to the brush on stand
            MoveLinear(new CartesianPosition(brushPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

            GripOff();

            // Move to position above the stand again
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

            CurrentBrush = -1;
        }

        /// <summary>
        /// Returns current held brush to the stand
        /// </summary>
        public void PickNewBrush(int num)
        {
            CurrentBrush = num;
            CartesianPosition brushPosition = _brushesLocations[num];
            Point brushPoint = brushPosition.Point;
            Point upperPoint = new Point(brushPoint.X, brushPoint.Y, brushPoint.Z + _brushLength);
            RPYMatrix orthogonalRPY = brushPosition.Rpymatrix;

            // Move to position above the brush
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

            Console.WriteLine($"PickNewBrush Upper Point: {new CartesianPosition(upperPoint, orthogonalRPY)}");

            GripOff();

            // Move to the brush on stand
            MoveLinear(new CartesianPosition(brushPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

            GripOn();

            // Move to position above the stand again
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 100, 25, MovementType.Absolute);
        }

        // Dunk current brush it the palette color
        public void DunkBrushInColor(CartesianPosition colorPosition)
        {
            Point colorPoint = colorPosition.Point;
            Point upperPoint = new Point(colorPoint.X, colorPoint.Y, colorPoint.Z + _brushLength);
            RPYMatrix orthogonalRPY = colorPosition.Rpymatrix;

            Console.WriteLine($"DunkBrushInColor Upper Point: {new CartesianPosition(upperPoint, orthogonalRPY)}");
            // Move to position above the palete
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

            // Move to color on palette
            MoveLinear(new CartesianPosition(colorPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

            // Move to position above the palete again
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 100, 25, MovementType.Absolute);
        }

        public void DryCurrentBrush()
        {
            Point dryerPoint = _dryerLocation.Point;
            Point upperPoint = new Point(dryerPoint.X, dryerPoint.Y, dryerPoint.Z + _brushLength);
            RPYMatrix orthogonalRPY = _dryerLocation.Rpymatrix;

            Console.WriteLine($"DryCurrentBrush Upper Point: {new CartesianPosition(upperPoint, orthogonalRPY)}");
            // Move to position above the dryer
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

            // Move to dryer
            MoveLinear(new CartesianPosition(dryerPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

            int rotationCount = 3;
            for (int i = 0; i < rotationCount; i++)
            {
                //double c = (i % 2) * 2 - 1;
                double c = Math.Pow(-1, i);
                JointMove(new JointsPosition(0, 0, 0, 0, 0, c * 30), 100, 100, MovementType.Relative);
            }

            // Move to position above the dryer again
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 100, 25, MovementType.Absolute);
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

        [Obsolete("Not recommended to use, consider using explicit methods GripOn and GripOff")]
        public void ToggleGrip()
        {
            _grip = !_grip;
            SetDOState(0, 0, _grip);
        }
    }
}
