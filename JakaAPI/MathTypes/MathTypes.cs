using System.Globalization;
using System.Text.Json.Serialization;

namespace JakaAPI.Types.Math
{
    public struct CartesianPosition
    {
        public Point Point { get; private set; }
        public RPYRotation Rpymatrix { get; private set; }

        [JsonConstructor]
        public CartesianPosition(Point point, RPYRotation rpymatrix)
        {
            Point = point;
            Rpymatrix = rpymatrix;
        }

        public CartesianPosition(double x, double y, double z, double rx, double ry, double rz)
        {
            Point = new Point(x, y, z);
            Rpymatrix = new RPYRotation(rx, ry, rz);
        }

        public CartesianPosition(double[] pos) : this(pos[0], pos[1], pos[2], pos[3], pos[4], pos[5])
        {
            if (pos.Length != 6)
            {
                throw new ArgumentException("Not enough positions");
            }
        }

        public override string ToString()
        {
            return
                $"[{Point.X.ToString(CultureInfo.InvariantCulture)}," +
                $"{Point.Y.ToString(CultureInfo.InvariantCulture)}," +
                $"{Point.Z.ToString(CultureInfo.InvariantCulture)}," +
                $"{Rpymatrix.Rx.ToString(CultureInfo.InvariantCulture)}," +
                $"{Rpymatrix.Ry.ToString(CultureInfo.InvariantCulture)}," +
                $"{Rpymatrix.Rz.ToString(CultureInfo.InvariantCulture)}]";
        }
    }

    /// <summary>
    /// A structure, which represents a Point
    /// </summary>
    public struct Point
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        [JsonConstructor]
        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"[{X.ToString(CultureInfo.InvariantCulture)}," +
                $"{Y.ToString(CultureInfo.InvariantCulture)}," +
                $"{Z.ToString(CultureInfo.InvariantCulture)}]";
        }
    }

    /// <summary>
    /// A structure, which represents Roll-Pitch-Yaw rotation (Tait-Bryan angles in degrees)
    /// </summary>
    public struct RPYRotation
    {
        public double Rx { get; private set; }
        public double Ry { get; private set; }
        public double Rz { get; private set; }

        [JsonConstructor]
        public RPYRotation(double rx, double ry, double rz)
        {
            Rx = rx;
            Ry = ry;
            Rz = rz;
        }

        public override string ToString() => $"Roll: {Rx}, Pitch: {Ry}, Yaw: {Rz}";
    }
}
