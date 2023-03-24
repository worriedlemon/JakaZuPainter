using JakaAPI.Types.Math;
using PainterArm.Calibration;
using System.Text.Json.Serialization;

namespace PainterArm
{
    /// <summary>
    /// Structure for representing a 2-dimentional coordinate system
    /// </summary>
    public class CoordinateSystem2D : ICalibratable
    {
        [JsonInclude]
        public Point Zero, AxisX, AxisY;

        [JsonInclude]
        public RPYMatrix RPYParameters;

        private Vector3 _axisX, _axisY, _zShift;
        private double _maxX, _maxY;

        public double UnitsPerMillimeter
        {
            get
            {
                return _upm;
            }
            set
            {
                double coeff = _upm / value;
                _axisX *= coeff;
                _axisY *= coeff;
                _zShift *= coeff;
                _maxX /= coeff;
                _maxY /= coeff;
                _upm = value;
            }
        }

        private double _upm = 1;

        /// <summary>
        /// Constructor for creating a new coordinate system in two dimentions (2D) based on 3 points
        /// </summary>
        /// <param name="zero">Point, representing the origin of the coordinate system</param>
        /// <param name="axisX">Point in the direction of X-axis</param>
        /// <param name="axisY">Point in the direction of Y-axis</param>
        /// <param name="RPYparameters">Rotation parameters of the grip</param>
        [JsonConstructor]
        public CoordinateSystem2D(Point zero, Point axisX, Point axisY, RPYMatrix RPYparameters, double unitsPerMillimeter = 1)
        {
            Zero = zero;
            AxisX = axisX;
            AxisY = axisY;
            RPYParameters = RPYparameters;

            _axisX = (Vector3)AxisX - (Vector3)Zero;
            _maxX = _axisX.Length();
            _axisX /= _maxX;
            _axisY = (Vector3)AxisY - (Vector3)Zero;
            _maxY = _axisY.Length();
            _axisY /= _maxY;

            _zShift = Vector3.VectorProduct(_axisX, _axisY, false);
            UnitsPerMillimeter = unitsPerMillimeter;
        }

        /// <summary>
        /// Converts a canvas point into world point using calibration points
        /// </summary>
        /// <param name="x">X position on a canvas in units</param>
        /// <param name="y">Y position on a canvas in units</param>
        /// <param name="z">Z shifting perpendicular to canvas</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Point CanvasPointToWorldPoint(double x, double y, double z = 0)
        {
            if (!(x >= 0 && x <= _maxX || y >= 0 && y <= _maxY)) throw new ArgumentException("X or Y out of field");
            return (Point)((Vector3)Zero + _axisX * x + _axisY * y + _zShift * z);
        }

        public override string ToString()
        {
            return $"Zero: {Zero}\nAxisX: {AxisX}\nAxisY: {AxisY}";
        }
    }

    /// <summary>
    /// Workaround structure for making location dictionary (of types <see cref="int"/> and <see cref="CartesianPosition"/>) possible to save and load
    /// </summary>
    public class LocationDictionary : Dictionary<int, CartesianPosition>, ICalibratable { }
}
