namespace PainterArm.Calibration
{
    public class AutomaticCalibration : AbstractCalibrationBehavior
    {
        public AutomaticCalibration(JakaPainter painterArm) : base(painterArm) { }

        public override CoordinateSystem2D Calibrate()
        {
            Console.WriteLine("---- [Surface calibration] ----\nStarted automatic calibration...");
            throw new NotImplementedException("Automatic calibration not implemented yet. Use ManualCalibration for now.");
        }
    }
}
