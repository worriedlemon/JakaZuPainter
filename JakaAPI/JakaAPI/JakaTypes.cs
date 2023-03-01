namespace JakaAPI
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
            this.j1 = j1; 
            this.j2 = j2;
            this.j3 = j3;
            this.j4 = j4;
            this.j5 = j5;
            this.j6 = j6;
        }

        public double j1 { get; private set; }
        public double j2 { get; private set; }
        public double j3 { get; private set; }
        public double j4 { get; private set; }
        public double j5 { get; private set; }
        public double j6 { get; private set; }

        public override string ToString()
        {
            return $"[{j1},{j2},{j3},{j4},{j5},{j6}]";
        }
    }

    public struct CartesianPosition
    {
        public CartesianPosition(double x, double y, double z, double raw, double pitch, double yaw)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.raw = raw;
            this.pitch = pitch;
            this.yaw = yaw;
        }

        public double x { get; private set; }
        public double y { get; private set; }
        public double z { get; private set; }
        public double raw { get; private set; }
        public double pitch { get; private set; }
        public double yaw { get; private set; }

        public override string ToString()
        {
            return $"[{x},{y},{z},{raw},{pitch},{yaw}]";
        }
    }

}