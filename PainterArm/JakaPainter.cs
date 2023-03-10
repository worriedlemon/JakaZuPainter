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

        private BrushesControl _brushConrol;

        private int _currentBrush = -1;

        /// <summary>
        /// Indicates whether the grip of the robot is being in grap state
        /// </summary>
        private bool _grip;

        public JakaPainter(string domain, int portSending = 10001, int portListening = 10000)
            : base(domain, portSending, portListening)
        {
            _brushConrol = new BrushesControl();

            //_grip = false;
            //SetDOState(0, 0, _grip);
        }

        public void StartCalibration()
        {
            Console.WriteLine("-- Painter Robot Calibration --");
            Console.WriteLine("Choose calibration option:\n 1) Canvas points\n 2) Palette colors\n 3) Water\n 4) Dryer");

            _isCalibrated = true;
        }

        public void CalibrateSurface(CalibrationPoint calibrationPoint)
        {
            switch (calibrationPoint)
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
            }
        }


        // Take avaliable brush with robot arm
        public void BrushTakeAvaliable()
        {
             Task.Delay(3000);
            Console.WriteLine("Brush taken");
        }

        // Take specific brush with robot arm
        public void BrushTake(int num)
        {
             Task.Delay(3000);
            Console.WriteLine($"Brush {num} taken");
        }

        // Take free brush and paint it with color on specific location
        public void BrushColor(CartesianPosition colorLocation)
        {
            if (_currentBrush != -1)
            {
                 Task.Delay(3000);
                Console.WriteLine("Brush colored");
            }
        }

        public void BrushWash()
        {
            if (_currentBrush != -1)
            {
                 Task.Delay(3000);
                Console.WriteLine("Brush washed");
            }
        }

        public void BrushDry()
        {
            if (_currentBrush != -1)
            {
                 Task.Delay(3000);
                Console.WriteLine("Brush dryed");
            }
        }

        public void BrushDrawLine(CartesianPosition target)
        {
             Task.Delay(3000);
            Console.WriteLine("Brush drew a line to target point");
        }

        // Experimental function for grip toggling
        public void ToggleGrip()
        {
            _grip = !_grip;
            SetDOState(0, 0, _grip);
        }
    }
}
