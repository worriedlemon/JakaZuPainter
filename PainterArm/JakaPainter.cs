using JakaAPI.Types;
using JakaAPI.Types.Math;
using JakaAPI;
using PainterArm.Calibration;
using PainterArm.MathExtensions;

namespace PainterArm
{
    /// <summary>
    /// Jaka Robot based implementation of painting robot
    /// </summary>
    public class JakaPainter : JakaRobot
    {
        private CoordinateSystem2D? _canvasCS;
        private double _currentX, _currentY, _currentHeight;

        private LocationDictionary _brushesLocations;
        private CartesianPosition _dryerLocation;

        public readonly double BrushLength = 95;
        private readonly double BrushAirOffset = 40;
        public readonly double NeedleLength = 95;

        private double _speed = 150;
        private double _acceleration = 100;

        private BrushPressStyle style = BrushPressStyle.Smooth;

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

        private Point prevPoint_2d = new Point(0, 0, 0);

        public JakaPainter(string domain, int portSending = 10001, int portListening = 10000)
            : base(domain, portSending, portListening)
        {
            _brushesLocations = new LocationDictionary();
            _grip = true;
            _currentX = 0;
            _currentY = 0;
            _currentHeight = 0;

            CurrentBrush = -1;
            //SetDOState(0, 0, _grip);
            CanvasCalibrationBehavior = new NeedleManualThreePointCalibration(this);
            DryerCalibrationBehavior = new ManualOnePointCalibration(this);
            BrushesCalibrationBehavior = new ManualOnePointCalibration(this);
        }

        /// <summary>
        /// Canvas calibration based on existing <see cref="CoordinateSystem2D"/>
        /// </summary>
        /// <param name="cs">Existing coordinate system to be used as canvas</param>
        public void CalibrateCanvas(CoordinateSystem2D cs) => _canvasCS = cs;

        /// <summary>
        /// Brushes calibration
        /// </summary>
        public void CalibrateBrushes(LocationDictionary locations) => _brushesLocations = locations;

        /// <summary>
        /// Dryer calibration
        /// </summary>
        public void CalibrateDryer(CartesianPosition location) => _dryerLocation = location;

        /// <summary>
        /// Moves brush in the air zone to a current position
        /// </summary>
        /// <param name="x">X-axis offset in millimeters <i>(or special units like 25 micron?)</i></param>
        /// <param name="y">Y-axis offset in millimeters</param>
        public void MoveBrushAir(double x, double y)
        {
            Console.WriteLine("MoveBrushAir");

            if (_currentHeight != BrushLength + BrushAirOffset)
            {
                Console.WriteLine("BrushOrthogonalMove");
                BrushOrthogonalMove(BrushLength + BrushAirOffset, MovementType.Absolute);
            }

            Console.WriteLine("MoveLinear");
            Point point3d = _canvasCS!.CanvasPointToWorldPoint(_currentX = x, _currentY = y, _currentHeight);
            MoveLinear(new CartesianPosition(point3d, _canvasCS.RPYParameters), _speed, _acceleration, MovementType.Absolute);
        }

        /// <summary>
        /// Draw line with canvas 2D coordinates with current style
        /// </summary>
        /// <param name="x">X-axis offset in millimeters <i>(or special units like 25 micron?)</i></param>
        /// <param name="y">Y-axis offset in millimeters</param>
        public void MoveLine(double x, double y, double z, double pressOfset = 3)
        {
            DrawLineConstantStyle(x, y, z, pressOfset);
        }

        int flag = 0;
        private void DrawTrapezoidStroke(double x, double y, double zPressOffset)
        {
            if (prevPoint_2d.X == 0 && prevPoint_2d.Y == 0 && prevPoint_2d.Z == 0) // Переписать
            {
                prevPoint_2d = new Point(x, y, 0);
                return;
            }

            Point p1 = prevPoint_2d;
            Point p4 = new Point(x, y, 0);

            Vector3 direction = (Vector3)p4 - (Vector3)p1;

            Point p2 = p1 + direction * 0.2;
            Point p3 = p1 + direction * 0.8;

            List<Point> points3d = new List<Point>();

            switch (flag)
            {
                case 0:
                    points3d.Add(_canvasCS!.CanvasPointToWorldPoint(p1 + new Vector3(0, 0, BrushLength - 1)));
                    points3d.Add(_canvasCS!.CanvasPointToWorldPoint(p4 + new Vector3(0, 0, BrushLength - 1)));
                    break;
                case 1:
                    points3d.Add(_canvasCS!.CanvasPointToWorldPoint(p1 + new Vector3(0, 0, BrushLength - 3)));
                    points3d.Add(_canvasCS!.CanvasPointToWorldPoint(p4 + new Vector3(0, 0, BrushLength - 3)));
                    break;
                case 2:
                    points3d.Add(_canvasCS!.CanvasPointToWorldPoint(p1 + new Vector3(0, 0, BrushLength)));
                    points3d.Add(_canvasCS!.CanvasPointToWorldPoint(p2 + new Vector3(0, 0, BrushLength - zPressOffset)));
                    points3d.Add(_canvasCS!.CanvasPointToWorldPoint(p3 + new Vector3(0, 0, BrushLength - zPressOffset)));
                    points3d.Add(_canvasCS!.CanvasPointToWorldPoint(p4 + new Vector3(0, 0, BrushLength)));
                    break;
            }

            flag++;

            foreach (Point p in points3d)
            {
                MoveLinear(new CartesianPosition(p, _canvasCS!.RPYParameters), _speed, _acceleration, MovementType.Absolute);
            }

            prevPoint_2d = p4;
        }

        private void DrawLineConstantStyle(double x, double y, double z, double pressOffset)
        {
            Point point3d = _canvasCS!.CanvasPointToWorldPoint(x, y, z + BrushLength - pressOffset);
            MoveLinear(new CartesianPosition(point3d, _canvasCS.RPYParameters), _speed, _acceleration, MovementType.Absolute);
        }

        private void DrawLineSmoothStyle(double x, double y, double zPressOffset)
        {
            // Рисование начинается выше точки
            BrushOrthogonalMove(BrushLength + zPressOffset, MovementType.Absolute);

            // Кисть нажимается, но не полностью
            BrushOrthogonalMove(BrushLength - zPressOffset / 2, MovementType.Absolute);

            // Кисть дивгается в точку, постепенно увеличивая степень нажатия до максимальной
            _currentHeight -= zPressOffset / 2;
            Point point3d = _canvasCS!.CanvasPointToWorldPoint(_currentX = x, _currentY = y, _currentHeight);
            MoveLinear(new CartesianPosition(point3d, _canvasCS.RPYParameters), _speed, _acceleration, MovementType.Absolute);

            // Кисть снова поднимается
            BrushOrthogonalMove(BrushLength + zPressOffset, MovementType.Absolute);
        }

        private void DrawLineAngular(double x, double y, double zPressOffset)
        {
            Console.WriteLine("\nDrawLineAngular");
            //BrushOrthogonalMove(BrushLength, MovementType.Absolute);

            double angleDeg = Math.PI / 4;
            if (prevPoint_2d.X == 0 && prevPoint_2d.Y == 0 && prevPoint_2d.Z == 0) // Переписать
            {
                prevPoint_2d = new Point(x, y, 0);
                Console.WriteLine("Изначальная точка записана: " + prevPoint_2d);
                return;
            }

            Point newPoint_2d = new Point(x, y, 0);
            Console.WriteLine("Новая точка встречена: " + newPoint_2d);

            ((Point p1, Point p2), RPYRotation rotation) pointsOffset = GetPointsOffset(prevPoint_2d, newPoint_2d, angleDeg);



            Point p1 = pointsOffset.Item1.p1;
            Point p2 = pointsOffset.Item1.p2;
            RPYRotation rot = pointsOffset.rotation;

            double height = p1.Z;

            Console.WriteLine($"height: {height}");

            Point p1_up = new Point(p1.X, p1.Y, p1.Z + zPressOffset);
            //MoveLinear(new CartesianPosition(p1_up, rot), _speed, _acceleration, MovementType.Absolute);

            //BrushOrthogonalMove(height - zPressOffset, MovementType.Absolute);

            //MoveLinear(new CartesianPosition(p2, rot), _speed, _acceleration, MovementType.Absolute);

            //BrushOrthogonalMove(BrushLength, MovementType.Absolute);

            prevPoint_2d = newPoint_2d;
        }

        private ((Point p1, Point p2), RPYRotation rotation) GetPointsOffset(Point prevPoint_2d, Point newPoint_2d, double angle)
        {
            double brushProjectionLength = BrushLength * Math.Cos(angle);
            Console.WriteLine($"brushProjectionLength: {brushProjectionLength}");

            Vector3 direction_2d = (Vector3)newPoint_2d - (Vector3)prevPoint_2d;
            Console.WriteLine($"direction_2d: {direction_2d}");

            Vector3 brushProjection = direction_2d.Normalized() * brushProjectionLength;
            Console.WriteLine($"brushProjection: {brushProjection}");

            Point heightProjection = prevPoint_2d + brushProjection;
            Console.WriteLine($"heightProjection: {heightProjection}");

            double height = BrushLength * Math.Sin(angle);
            Console.WriteLine($"height: {height}");

            Point heightPoint = heightProjection + new Vector3(0, 0, height);
            Console.WriteLine($"heightPoint: {heightPoint}");

            Vector3 brushDirection = (Vector3)prevPoint_2d - (Vector3)heightPoint;
            Console.WriteLine($"brushDirection: {brushDirection}");

            Point point1_3d = _canvasCS!.CanvasPointToWorldPoint(heightPoint);
            Console.WriteLine($"point1_3d: {point1_3d}");

            Point point2_3d = _canvasCS!.CanvasPointToWorldPoint(heightPoint + direction_2d);
            Console.WriteLine($"point2_3d: {point2_3d}");

            RPYRotation rotation = ((Vector3)_canvasCS.CanvasPointToWorldPoint(prevPoint_2d) - (Vector3)point1_3d).Normalized().ToRPY().MainSolution;
            Console.WriteLine($"rotation: {rotation}");


            return ((point1_3d, point2_3d), rotation);
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
            double _newHeight = (movementType == MovementType.Relative) ? _currentHeight + height : height;

            if (_newHeight != _currentHeight)
            {
                _currentHeight = _newHeight;

                Point point3d = _canvasCS!.CanvasPointToWorldPoint(_currentX, _currentY, _currentHeight);

                MoveLinear(new CartesianPosition(point3d, _canvasCS.RPYParameters), _speed, _acceleration, MovementType.Absolute);
            }
        }

        /// <summary>
        /// Returns current held brush to the stand
        /// </summary>
        public void ReturnCurrentBrush()
        {
            CartesianPosition brushPosition = _brushesLocations[CurrentBrush];
            Point brushPoint = brushPosition.Point;
            Point upperPoint = new(brushPoint.X, brushPoint.Y, brushPoint.Z + BrushLength + BrushAirOffset);
            RPYRotation orthogonalRPY = brushPosition.Rpymatrix;

            // Move to position above the brush
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), _speed, _acceleration, MovementType.Absolute);

            // Move to the brush on stand
            MoveLinear(new CartesianPosition(brushPoint, orthogonalRPY), _speed, _acceleration, MovementType.Absolute);

            // Rotate the brush
            RotateBrush(1, 30, false);

            // Move to position above the stand again
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), _speed, _acceleration, MovementType.Absolute);

            CurrentBrush = -1;

            if (GetBrushState(CurrentBrush) != BrushSlotState.Occupied)
            {
                throw new Exception("Brush was not actually returned");
            }
        }

        /// <summary>
        /// Takes new brush to the stand
        /// </summary>
        public void PickNewBrush(int brushNum)
        {
            CurrentBrush = brushNum;
            CartesianPosition brushPosition = _brushesLocations[brushNum];
            Point brushPoint = brushPosition.Point;
            Point upperPoint = new Point(brushPoint.X, brushPoint.Y, brushPoint.Z + BrushLength + BrushAirOffset);
            RPYRotation orthogonalRPY = brushPosition.Rpymatrix;

            // Move to position above the brush
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), _speed, _acceleration, MovementType.Absolute);

            // Move to the brush on stand
            MoveLinear(new CartesianPosition(brushPoint, orthogonalRPY), _speed, _acceleration, MovementType.Absolute);

            // Rotate the brush
            RotateBrush(1, 30, true);

            // Move to position above the stand again
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), _speed, _acceleration, MovementType.Absolute);

            if (GetBrushState(brushNum) != BrushSlotState.Empty)
            {
                throw new Exception("Brush was not actually taken");
            }
        }

        /// <summary>
        /// Dunks taken brush in the provided position
        /// </summary>
        public void DunkBrushInColor(CartesianPosition colorPosition)
        {
            double offset = 5;

            Point colorPoint = colorPosition.Point;

            Point upperPoint = new Point(colorPoint.X, colorPoint.Y, colorPoint.Z + BrushLength - offset);
            Point airPoint = new Point(colorPoint.X, colorPoint.Y, colorPoint.Z + BrushLength + BrushAirOffset);
            RPYRotation orthogonalRPY = colorPosition.Rpymatrix;

            // Move to position above the palete
            MoveLinear(new CartesianPosition(airPoint, orthogonalRPY), _speed, _acceleration, MovementType.Absolute);

            // Move to color on palette
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), _speed, _acceleration, MovementType.Absolute);

            Thread.Sleep(50);

            // Move to position above the palete again
            MoveLinear(new CartesianPosition(airPoint, orthogonalRPY), _speed, _acceleration, MovementType.Absolute);
        }

        /// <summary>
        /// Dryes current brush in the dryer
        /// </summary>
        public void DryCurrentBrush()
        {
            Point dryerPoint = _dryerLocation.Point;
            Point upperPoint = new Point(dryerPoint.X, dryerPoint.Y, dryerPoint.Z + BrushAirOffset);
            RPYRotation orthogonalRPY = _dryerLocation.Rpymatrix;

            // Move to position above the dryer
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), _speed, _acceleration, MovementType.Absolute);

            // Move to dryer
            MoveLinear(new CartesianPosition(dryerPoint, orthogonalRPY), _speed, _acceleration, MovementType.Absolute);

            // Time for washing
            Thread.Sleep(2000);

            // Move to position above the dryer again
            MoveLinear(new CartesianPosition(upperPoint, orthogonalRPY), _speed, _acceleration, MovementType.Absolute);
        }

        /// <summary>
        /// Rotates the brush with given arguments
        /// </summary>
        private void RotateBrush(int rotationCount, double angle, bool clockwise)
        {
            for (int i = 0; i < rotationCount; i++)
            {
                double c = Math.Pow(-1, i);
                c *= clockwise ? 1 : -1;
                JointMove(new JointsPosition(0, 0, 0, 0, 0, c * angle), _speed, _acceleration, MovementType.Relative);
            }
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
        /// <returns><see cref="BrushSlotState.Empty"/> if input contains high voltage signal, <see cref="BrushSlotState.Occupied"/> otherwise</returns>
        public BrushSlotState GetBrushState(int brushNum)
        {
            bool[] states = GetDIStatus();
            return states[_brushesDI[brushNum]] ? BrushSlotState.Empty : BrushSlotState.Occupied;
        }

        /// <summary>
        /// Enabled/disables brush holder checker by applying state to specific pin
        /// </summary>
        /// <param name="enable">If true, checker will be enabled, else it will be disabled</param>
        public void SetBrushHolderCheckingState(bool enable) => SetDOState(0, 9, enable);
    }
}
