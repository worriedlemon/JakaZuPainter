using JakaAPI.Types.Math;

namespace PainterArm.Calibration
{
    public class ManualOnePointCalibration : AbstractCalibrationBehavior
    {
        public ManualOnePointCalibration(in JakaPainter painterArm) : base(painterArm) { }

        public override LocationDictionary Calibrate()
        {
            Console.WriteLine("(1) Set new location\n(0) End calibration");
            LocationDictionary locations = new();

            while (true)
            {
                int option = Int32.Parse(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        locations.Add(locations.Count, PainterArm.GetRobotData().ArmCartesianPosition);
                        break;
                    case 0:
                        return locations;
                }
            };
        }
    }
}
