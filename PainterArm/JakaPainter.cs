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

        private const double _brushLength = 95;
        private const double _needleLength = 95;

        public int CurrentBrush { get; private set; }

        private readonly Dictionary<int, int> _brushesDI = new()
            {
                { 0, 9 }, // Change to actual DIs
                { 1, 10 },
                { 2, 11 },
                { 3, 12 },
                { 4, 13 },
                { 5, 14 },
            };

        /// <summary>
        /// Indicates whether the grip of the robot is being in grap state
        /// </summary>
        private bool _grip;

        public AbstractCalibrationBehavior CanvasCalibrationBehavior, BrushesCalibrationBehavior, DryerCalibrationBehavior;

        public JakaPainter(string domain, int portSending = 10001, int portListening = 10000)
            : base(domain, portSending, portListening)
        {
            _brushesLocations = new LocationDictionary();
            _grip = true;
            _currentX = 0;
            _currentY = 0;
            _currentHeight = 0;

            CurrentBrush = -1;
            SetDOState(0, 0, _grip);
            CanvasCalibrationBehavior = new NeedleManualThreePointCalibration(this, _needleLength);
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
            MoveLinear(new CartesianPosition(point3d, _canvasCoordinateSystem.RPYParameters), 200, 45, MovementType.Absolute);
        }

        // Raw method, will be implemented soon
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
        /// <param name="movementType">Brush movement type, absolute or relative</param>
        public void BrushOrthogonalMove(double height, MovementType movementType)
        {
            _currentHeight = (movementType == MovementType.Relative) ? _currentHeight + height : height;

            Point point3d = _canvasCoordinateSystem!.CanvasPointToWorldPoint(_currentX, _currentY, _currentHeight);

            MoveLinear(new CartesianPosition(point3d, _canvasCoordinateSystem.RPYParameters), 100, 25, MovementType.Absolute);
        }

        /// <summary>
        /// Returns current held brush to the stand
        /// </summary>
        public void ReturnCurrentBrush()
        {
            CartesianPosition brushPosition = _brushesLocations[CurrentBrush];
            Point brushPoint = brushPosition.Point;
            Point upperPoint = new(brushPoint.X, brushPoint.Y, brushPoint.Z + _brushLength);
            RPYRotation orthogonalRPY = brushPosition.Rpymatrix;

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
            RPYRotation orthogonalRPY = brushPosition.Rpymatrix;

            // Move to position above the brush
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

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
            RPYRotation orthogonalRPY = colorPosition.Rpymatrix;

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
            RPYRotation orthogonalRPY = _dryerLocation.Rpymatrix;

            // Move to position above the dryer
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

            // Move to dryer
            MoveLinear(new CartesianPosition(dryerPoint, orthogonalRPY), 100, 25, MovementType.Absolute);

            int rotationCount = 3;
            for (int i = 0; i < rotationCount; i++)
            {
                double c = Math.Pow(-1, i);
                JointMove(new JointsPosition(0, 0, 0, 0, 0, c * 30), 100, 100, MovementType.Relative);
            }

            // Move to position above the dryer again
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), 100, 25, MovementType.Absolute);
        }

        /// <summary>
        /// Sets the state of the grip to <b>ON</b>
        /// </summary>
        public void GripOn()
        {
            _grip = true;
            SetDOState(0, 0, _grip);
        }

        /// <summary>
        /// Sets the state of the grip to <b>OFF</b>
        /// </summary>
        public void GripOff()
        {
            _grip = false;
            SetDOState(0, 0, _grip);
        }

        /// <summary>
        /// Toggles the state of the grip
        /// </summary>
        [Obsolete("Not recommended to use, consider using explicit methods GripOn and GripOff")]
        public void ToggleGrip()
        {
            _grip = !_grip;
            SetDOState(0, 0, _grip);
        }

        /// <summary>
        /// Getting brush slot state based on Hall sensor:<br/>
        /// - <i>High voltage</i> means <i>no magnetic field</i>, a.k.a. no brush in the slot<br/>
        /// - <i>Low voltage</i> means <i>magnetic field presence</i>, a.k.a. brush is in the slot 
        /// </summary>
        /// <param name="brushNum"></param>
        /// <returns><see cref="BrushSlotState.EMPTY"/> if input contains high voltage signal, <see cref="BrushSlotState.OCCUPIED"/> otherwise</returns>
        public BrushSlotState GetBrushState(int brushNum)
        {
            bool[] states = GetDIStatus();
            return states[_brushesDI[brushNum]] ? BrushSlotState.EMPTY : BrushSlotState.OCCUPIED;
        }

        /// <summary>
        /// Enabled/disables brush holder checker by applying state to specific pin
        /// </summary>
        /// <param name="enable">If true, checker will be enabled, else it will be disabled</param>
        public void SetBrushHolderCheckingState(bool enable) => SetDOState(0, 9, enable);
    }
}
