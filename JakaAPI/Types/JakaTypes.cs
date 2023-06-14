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
            return 
               $"[{J1.ToString(CultureInfo.InvariantCulture)}," +
               $"{J2.ToString(CultureInfo.InvariantCulture)}," +
               $"{J3.ToString(CultureInfo.InvariantCulture)}," +
               $"{J4.ToString(CultureInfo.InvariantCulture)}," +
               $"{J5.ToString(CultureInfo.InvariantCulture)}," +
               $"{J6.ToString(CultureInfo.InvariantCulture)}]";
        }
    }
}
