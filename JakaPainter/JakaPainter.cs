using JakaAPI.Types;

namespace JakaAPI
{
    /// <summary>
    /// Jaka Robot based implementation of painting robot
    /// </summary>
    public class JakaPainter : JakaRobot
    {
        private Point[] _surfacePoints;

        private bool _isCalibrated = false;

        public JakaPainter(string domain, int portSending = 10001, int portListening = 10000) : base(domain, portSending, portListening) 
        {
            _surfacePoints = new Point[3];
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
                case CalibrationPoint.LeftTop:
                    break;
                case CalibrationPoint.LeftBottom:
                    break;
                case CalibrationPoint.RightBottom:
                    break;
            }
        }

        public void BrushWash()
        {

        }

        public void BrushDry()
        {

        }
    }
}
