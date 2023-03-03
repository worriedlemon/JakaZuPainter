using JakaAPI.Types;

namespace JakaAPI
{
    public class JakaPainter : JakaRobot
    {
        private Point[] _surfacePoints;

        public JakaPainter(string domain, int port = 10001) : base(domain, port) 
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
