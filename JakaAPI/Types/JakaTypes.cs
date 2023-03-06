using System.Globalization;

namespace JakaAPI.Types
{
    public enum MovementType
    {
        Absolute = 0,
        Relative = 1
    }

    public struct JointsPosition
    {
        public double J1 { get; private set; }
        public double J2 { get; private set; }
        public double J3 { get; private set; }
        public double J4 { get; private set; }
        public double J5 { get; private set; }
        public double J6 { get; private set; }

        public JointsPosition(double j1, double j2, double j3, double j4, double j5, double j6)
        {
            J1 = j1;
            J2 = j2;
            J3 = j3;
            J4 = j4;
            J5 = j5;
            J6 = j6;
        }

        public JointsPosition(double[] joints) : this(joints[0], joints[1], joints[2], joints[3], joints[4], joints[5])
        {
            if (joints.Length != 6)
            {
                throw new ArgumentException("Not enough joints positions");
            }
        }

        public override string ToString()
        {
            return $"[{J1},{J2},{J3},{J4},{J5},{J6}]";
        }
    }

    public struct CartesianPosition
    {
        public Point Point { get; private set; }
        public RPYMatrix Rpymatrix { get; private set; }

        public CartesianPosition(Point point, RPYMatrix rpymatrix)
        {
            Point = point;
            Rpymatrix = rpymatrix;
        }

        public CartesianPosition(double x, double y, double z, double rx, double ry, double rz)
        {
            Point = new Point(x, y, z);
            Rpymatrix = new RPYMatrix(rx, ry, rz);
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

    public struct Point
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Vector3(Point point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }
    }

    public struct Vector3
    {
        public double Dx { get; private set; }
        public double Dy { get; private set; }
        public double Dz { get; private set; }

        public Vector3(double x, double y, double z)
        {
            Dx = x;
            Dy = y;
            Dz = z;
        }

        public static Vector3 operator +(Vector3 first, Vector3 second)
        {
            return new Vector3(first.Dx + second.Dx, first.Dy + second.Dy, first.Dx + second.Dx);
        }

        public static Vector3 operator -(Vector3 first, Vector3 second)
        {
            return new Vector3(first.Dx - second.Dx, first.Dy - second.Dy, first.Dx - second.Dx);
        }

        public static Vector3 operator *(Vector3 vector, double coeff)
        {
            return new Vector3(vector.Dx * coeff, vector.Dy * coeff, vector.Dz * coeff);
        }

        public static implicit operator Point(Vector3 vector)
        {
            return new Point(vector.Dx, vector.Dy, vector.Dz);
        }

        public Vector3 Normalized()
        {
            double length = Math.Sqrt(Dx * Dx + Dy * Dy + Dz * Dz);
            if (length == 0)
            {
                throw new DivideByZeroException("Length of vector is zero");
            }
            return new Vector3(Dx / length, Dy / length, Dz / length);
        }
    }

    public struct RPYMatrix
    {
        public double Rx { get; private set; }
        public double Ry { get; private set; }
        public double Rz { get; private set; }

        public RPYMatrix(double rx, double ry, double rz)
        {
            Rx = rx;
            Ry = ry;
            Rz = rz;
        }
    }
}
