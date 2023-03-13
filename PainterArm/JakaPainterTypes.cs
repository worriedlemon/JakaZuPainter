using JakaAPI.Types.Math;

namespace PainterArm
{
    /// <summary>
    /// Structure for representing a canvas coordinate system
    /// </summary>
    public class CoordinateSystem2D
    {
        public Point Zero, AxisX, AxisY;

        public RPYMatrix CanvasRPY = new(0, 0, 0);

        private readonly Vector3 _axisX, _axisY;
        private readonly double _maxX, _maxY;

        private Vector3? _zShift;

        /// <summary>
        /// Constructor for creating a new coordinate system in two dimentions (2D) based on 3 points
        /// </summary>
        /// <param name="zero"></param>
        /// <param name="axisX"></param>
        /// <param name="axisY"></param>
        public CoordinateSystem2D(Point zero, Point axisX, Point axisY)
        {
            Zero = zero;
            AxisX = axisX;
            AxisY = axisY;

            _axisX = (Vector3)AxisX - (Vector3)Zero;
            _maxX = _axisX.Length();
            _axisX /= _maxX;
            _axisY = (Vector3)AxisY - (Vector3)Zero;
            _maxY = _axisY.Length();
            _axisY /= _maxY;

            _zShift = null;
        }

        /// <summary>
        /// Function for enabling a Z-axis shifting (orthogonal to X and Y)
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
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
            return (Point)((Vector3)Zero + _axisX * x + _axisY * y);
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
            return (Point)((Vector3)CanvasPointToWorldPoint(x, y) + (_zShift * z));
        }
    }
}
