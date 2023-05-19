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
        public RPYRotation RPYParameters;

        private Vector3 _axisX, _axisY, _zShift;

        [JsonIgnore]
        public double MaxX { get; private set; }
        [JsonIgnore]
        public double MaxY { get; private set; }

        [JsonInclude]
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
                MaxX /= coeff;
                MaxY /= coeff;
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
        public CoordinateSystem2D(Point zero, Point axisX, Point axisY, RPYRotation RPYparameters, double unitsPerMillimeter = 1)
        {
            Zero = zero;
            AxisX = axisX;
            AxisY = axisY;
            RPYParameters = RPYparameters;

            _axisX = (Vector3)AxisX - (Vector3)Zero;
            MaxX = _axisX.Length();
            _axisX /= MaxX;
            _axisY = (Vector3)AxisY - (Vector3)Zero;
            MaxY = _axisY.Length();
            _axisY /= MaxY;

            _zShift = Vector3.VectorProduct(_axisX, _axisY).Normalized();
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
            if (!(x >= 0 && x <= MaxX || y >= 0 && y <= MaxY)) throw new ArgumentException("X or Y out of field");
            return (Point)((Vector3)Zero + _axisX * x + _axisY * y + _zShift * z);
        }

        public static Vector3 FixZShiftByPoint(Vector3 zShift, Point zero, Point directionPoint)
        {
            return (Vector3.DotProduct((Vector3)directionPoint - (Vector3)zero, zShift) > 0) ? zShift : -zShift;
        }

        public override string ToString()
        {
            return $"Zero: {Zero}\nAxisX: {AxisX}\nAxisY: {AxisY}";
        }
    }

    /// <summary>
    /// Workaround structure for making location dictionary (of types <see cref="int"/> and <see cref="CartesianPosition"/>) possible to save and load
    /// </summary>
    public class LocationDictionary : Dictionary<int, CartesianPosition>, ICalibratable
    {
        public override string ToString()
        {
            string representation = "";
            foreach (var pair in this)
            {
                representation += $"Location {pair.Key}: {pair.Value}\n";
            }
            return representation;
        }
    }
}
