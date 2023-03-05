using JakaAPI.Types;

namespace JakaAPI
{
    /// <summary>
    /// Jaka Robot based implementation of painting robot
    /// </summary>
    public class JakaPainter : JakaRobot
    {
        private Point[] _surfacePoints;

        public JakaPainter(string domain, int portSending = 10001, int portListening = 10000) : base(domain, portSending, portListening) 
        {
            _surfacePoints = new Point[3];
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
    }
}
