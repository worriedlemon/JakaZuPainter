using JakaAPI.Types.Math;

namespace PainterArm
{
    public enum CalibrationPoint
    {
        LeftTop,
        LeftBottom,
        RightBottom,
    }

    /// <summary>
    /// Structure for representing a canvas coordinate system
    /// </summary>
    public struct CoordinateSystem2D
    {
        public Point Zero;

        public Vector3 AxisX
        {
            get
            {
                return _axisX * _maxX;
            }
            set
            {
                _maxX = value.Length();
                _axisX = value.Normalized();
            }
        }

        public Vector3 AxisY
        {
            get
            {
                return _axisY * _maxY;
            }
            set
            {
                _maxY = value.Length();
                _axisY = value.Normalized();
            }
        }

        public RPYMatrix CanvasRPY;

        private Vector3 _axisX, _axisY;
        private double _maxX, _maxY;
        private Vector3? _zShift;

        /// <summary>
        /// Function for enabling a Z-axis shifting (orthogonal to X and Y)
        /// </summary>
        public void UseZShifting()
        {
            _zShift = Vector3.VectorProduct(_axisX, _axisY, false);
        }

        /// <summary>
        /// Converts a canvas point into world point using calibration points
        /// </summary>
        /// <param name="x">X position on a canvas in millimeters</param>
        /// <param name="y">Y position on a canvas in millimeters</param>
        /// <returns>A world <see cref="Point"/> of a canvas point for robot to understand its position</returns>
        /// <exception cref="ArgumentException"></exception>
        public Point CanvasPointToWorldPoint(double x, double y)
        {
            if (!(x >= 0 && x <= _maxX || y >= 0 && y <= _maxY)) throw new ArgumentException("X or Y out of field");
            return Zero + (_axisX - Zero) * x + (_axisY - Zero) * y;
        }

        /// <summary>
        /// Converts a canvas point into world point using calibration points (including Z-shifting)
        /// </summary>
        /// <param name="x">X position on a canvas in millimeters</param>
        /// <param name="y">Y position on a canvas in millimeters</param>
        /// <param name="z">Z shifting perpendicular to canvas</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Point CanvasPointToWorldPoint(double x, double y, double z)
        {
            if (_zShift == null) throw new InvalidOperationException("Z is not used in this context");
            return CanvasPointToWorldPoint(x, y) + (_zShift * z);
        }
    }
}
