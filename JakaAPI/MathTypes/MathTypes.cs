using static System.Math;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Security.Cryptography;

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

        public static Vector3 operator +(Vector3 first, Vector3 second)
        {
            return new Vector3(first.Dx + second.Dx, first.Dy + second.Dy, first.Dz + second.Dz);
        }

        public static Vector3 operator -(Vector3 vector) => new Vector3(-vector.Dx, -vector.Dy, -vector.Dz);

        public static Vector3 operator -(Vector3 first, Vector3 second) => first + (-second);

        public static Vector3 operator *(Vector3 vector, double multiplier)
        {
            return new Vector3(vector.Dx * multiplier, vector.Dy * multiplier, vector.Dz * multiplier);
        }

        public static Vector3 operator /(Vector3 vector, double divider) => vector * (1.0 / divider);

        public static explicit operator Point(Vector3 vector) => new Point(vector.Dx, vector.Dy, vector.Dz);

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
        public static double DotProduct(Vector3 a, Vector3 b) => a.Dx * b.Dx + a.Dy * b.Dy + a.Dz * b.Dz;

        public override string ToString()
        {
            return $"[{Dx.ToString(CultureInfo.InvariantCulture)}," +
                $"{Dy.ToString(CultureInfo.InvariantCulture)}," +
                $"{Dz.ToString(CultureInfo.InvariantCulture)}]";
        }
    }

    public class Matrix3x3
    {
        private readonly double[,] data = new double[3, 3];

        public Matrix3x3(double[,] data)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    this.data[i, j] = data[i, j];
                }
            }
        }

        public Matrix3x3(double value = 0)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    data[i, j] = value;
                }
            }
        }

        public Matrix3x3(Vector3 x, Vector3 y, Vector3 z)
        {
            data[0, 0] = x.Dx;
            data[1, 0] = x.Dy;
            data[2, 0] = x.Dz;
            data[0, 1] = y.Dx;
            data[1, 1] = y.Dy;
            data[2, 1] = y.Dz;
            data[0, 2] = z.Dx;
            data[1, 2] = z.Dy;
            data[2, 2] = z.Dz;
        }

        public Vector3 this[int i] => new(data[0, i], data[1, i], data[2, i]);

        public double this[int i, int j] => data[i, j];

        public static Matrix3x3 operator *(Matrix3x3 A, Matrix3x3 B)
        {
            Matrix3x3 result = new();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        result.data[i, j] += A.data[i, k] * B.data[k, j];
                    }
                }
            }

            return result;
        }

        public static Matrix3x3 operator *(Matrix3x3 A, double value)
        {
            Matrix3x3 result = new();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result.data[i, j] = A.data[i, j] * value;
                }
            }

            return result;
        }

        public static Matrix3x3 operator *(double value, Matrix3x3 A) => A * value;

        public static readonly Matrix3x3 Identity = new(new double[3, 3]
                {
                    { 1, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 1 }
                });

        public static Vector3 operator*(Matrix3x3 m, Vector3 v)
        {
            double[] vc = new double[3] { 0, 0, 0 };
            double[] coord = new double[3] { v.Dx, v.Dy, v.Dz };
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    vc[i] += m.data[i, j] * coord[j];
                }
            }
            return new Vector3(vc[0], vc[1], vc[2]);
        }

        public static Matrix3x3 RotationMatrix(double rx, double ry, double rz, bool degrees = true)
        {
            DoubleTranslation translate = degrees ? MathDefinitions.DegToRad : (double arg) => { return arg; };
            (double sin, double cos) psi = SinCos(translate(rz)), theta = SinCos(translate(ry)), phi = SinCos(translate(rx));

            // Maybe should be transposed
            return new(new double[3, 3]
            {
                { psi.cos * theta.cos, psi.sin * theta.cos, -theta.sin },
                { psi.cos * theta.sin * phi.sin - psi.sin * phi.cos, psi.sin * theta.sin * phi.sin + psi.cos * phi.cos, theta.cos * phi.sin },
                { psi.cos * theta.sin * phi.cos + psi.sin * phi.sin, psi.sin * theta.sin * phi.cos - psi.cos * phi.sin, theta.cos * phi.cos }
            });
        }

        public RPYRotation ToRPY(bool degrees = true)
        {
            DoubleTranslation translate = degrees ? MathDefinitions.RadToDeg : (double arg) => { return arg; };

            double phi = Atan2(data[1, 2], data[2, 2]);
            double rx = translate(phi);
            //sign shadowing due to Sqrt (?)
            //double ry = translate(Atan2(-data[0, 2], Sqrt(data[0, 0] * data[0, 0] + data[0, 1] * data[0, 1])))
            double rz = translate(Atan2(data[0, 1], data[0, 0]));

            // Probably should also consider the case of data[2, 2] and data[1, 2] being zero (can it really be?)
            double ry;
            if (-1e-9 < phi && phi < 1e-9 || -1e-9 < phi - PI && phi - PI < 1e-9)
            {
                ry = translate(Atan2(-data[0, 2], data[2, 2] / Cos(phi)));
            }
            else ry = translate(Atan2(-data[0, 2], data[1, 2] / Sin(phi)));

            return new(rx, ry, rz);
        }

        public override string ToString()
        {
            return $"([{data[0, 0]}, {data[0, 1]}, {data[0, 2]}],\n[{data[1, 0]}, {data[1, 1]}, {data[1, 2]}],\n[{data[2, 0]}, {data[2, 1]}, {data[2, 2]}])";
        }
    }

    /// <summary>
    /// A structure, which represents a roll, pitch, yaw rotation matrix
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

    public static class MathDefinitions
    {
        public static double DegToRad(double value) => value * PI / 180;
        
        public static double RadToDeg(double value) => value * 180 / PI;
    }
}
