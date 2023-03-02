namespace JakaAPI.Types
{
    public enum MovementType
    {
        Absolute,
        Relative,
    }

    public struct JointPositions
    {
        public JointPositions(double j1, double j2, double j3, double j4, double j5, double j6)
        {
            J1 = j1;
            J2 = j2;
            J3 = j3;
            J4 = j4;
            J5 = j5;
            J6 = j6;
        }

        public double J1 { get; private set; }
        public double J2 { get; private set; }
        public double J3 { get; private set; }
        public double J4 { get; private set; }
        public double J5 { get; private set; }
        public double J6 { get; private set; }

        public override string ToString()
        {
            return $"[{J1},{J2},{J3},{J4},{J5},{J6}]";
        }
    }

    public struct CartesianPosition
    {
        public CartesianPosition(Point point, RPYmatrix rpymatrix)
        {
            Point = point;
            Rpymatrix = rpymatrix;
        }

        public CartesianPosition(double x, double y, double z, double rx, double ry, double rz)
        {
            Point = new Point(x, y, z);
            Rpymatrix = new RPYmatrix(rx, ry, rz);
        }
       
        public Point Point { get; private set; }

        public RPYmatrix Rpymatrix { get; private set; }

        public override string ToString()
        {
            return $"[{Point.X},{Point.Y},{Point.Z},{Rpymatrix.Rx},{Rpymatrix.Ry},{Rpymatrix.Rz}]";
        }
    }

    public struct Point
    {
        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; private set; }

        public double Y { get; private set; }

        public double Z { get; private set; }
    }

    public struct RPYmatrix
    {
        public RPYmatrix(double rx, double ry, double rz)
        {
            Rx = rx;
            Ry = ry;
            Rz = rz;
        }
        public double Rx { get; private set; }
        public double Ry { get; private set; }
        public double Rz { get; private set; }
    }
}
