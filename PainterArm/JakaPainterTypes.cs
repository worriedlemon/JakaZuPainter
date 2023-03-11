using JakaAPI.Types;

namespace PainterArm
{
    public enum CalibrationPoint
    {
        LeftTop,
        LeftBottom,
        RightBottom,
    }

    public struct CoordinateSystem2D
    {
        public Vector3 Zero;
        public Vector3 AxisX;
        public Vector3 AxisY;
        public RPYMatrix CanvasRPY;

        public Point Point2DToRealPoint(double x, double y)
        {
            Vector3 vectorX = (AxisX - Zero).Normalized();
            Vector3 vectorY = (AxisY - Zero).Normalized();
            return Zero + vectorX * x + vectorY * y;
        }
    }
}
