using static System.Math;
using System.Globalization;
using System.Text.Json.Serialization;

namespace JakaAPI.Types.Math
{
    using DoubleTranslation = Func<double, double>;

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

        public static explicit operator Vector3(Point point) => new(point.X, point.Y, point.Z);

        public override string ToString()
        {
            return $"[{X.ToString(CultureInfo.InvariantCulture)}," +
                $"{Y.ToString(CultureInfo.InvariantCulture)}," +
                $"{Z.ToString(CultureInfo.InvariantCulture)}]";
        }
    }

    /// <summary>
    /// A structure, which represents a geometric vector in 3 dimentions
    /// </summary>
    public class Vector3
    {
        public double Dx { get; private set; }
        public double Dy { get; private set; }
        public double Dz { get; private set; }

        public Vector3() { }

        public Vector3(double x, double y, double z)
        {
            Dx = x;
            Dy = y;
            Dz = z;
        }

        public static Vector3 operator +(Vector3 first, Vector3 second) => new(first.Dx + second.Dx, first.Dy + second.Dy, first.Dz + second.Dz);

        public static Vector3 operator -(Vector3 vector) => new(-vector.Dx, -vector.Dy, -vector.Dz);

        public static Vector3 operator -(Vector3 first, Vector3 second) => first + (-second);

        public static Vector3 operator *(Vector3 vector, double multiplier) => new(vector.Dx * multiplier, vector.Dy * multiplier, vector.Dz * multiplier);

        public static Vector3 operator *(double multiplier, Vector3 vector) => vector * multiplier;

        public static Vector3 operator /(Vector3 vector, double divider) => vector * (1.0 / divider);

        public static explicit operator Point(Vector3 vector) => new(vector.Dx, vector.Dy, vector.Dz);

        /// <returns>The length of this <see cref="Vector3"> instance</returns>
        public double Length() => Sqrt(Dx * Dx + Dy * Dy + Dz * Dz);

        /// <returns>A <see cref="Vector3"/> with the unit length and the same direction as the base</returns>
        /// <exception cref="DivideByZeroException"></exception>
        public Vector3 Normalized()
        {
            double length = Length();
            if (length == 0)
            {
                throw new DivideByZeroException("Length of vector is zero: cannot be normalized");
            }
            return this / length;
        }

        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="needNormalization"><see cref="Boolean"> to set whether vectors should be normalized in advance</param>
        /// <returns>A new <see cref="Vector3"/>, which is directed perpendicular to <i>a</i> and <i>b</i></returns>
        public static Vector3 VectorProduct(Vector3 a, Vector3 b) => new(a.Dy * b.Dz - a.Dz * b.Dy, a.Dz * b.Dx - a.Dx * b.Dz, a.Dx * b.Dy - a.Dy * b.Dx);

        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>A <see cref="double"/> value, representing dot product of two vectors</returns>
        public static double DotProduct(Vector3 a, Vector3 b) => a.Dx * b.Dx + a.Dy * b.Dy + a.Dz * b.Dz;

        public override string ToString()
        {
            return $"[{Dx.ToString(CultureInfo.InvariantCulture)}," +
                $"{Dy.ToString(CultureInfo.InvariantCulture)}," +
                $"{Dz.ToString(CultureInfo.InvariantCulture)}]";
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
