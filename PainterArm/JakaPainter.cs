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

        /// <summary>
        /// Indicates whether the grip of the robot is being in grap state
        /// </summary>
        private bool _grip;

        public JakaPainter(string domain, int portSending = 10001, int portListening = 10000)
            : base(domain, portSending, portListening)
        {
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

        public void BrushWash()
        {

        }

        public void BrushDry()
        {

        }

        // Experimental function for grip toggling
        public void ToggleGrip()
        {
            _grip = !_grip;
            SetDOState(0, 0, _grip);
        }
    }
}
