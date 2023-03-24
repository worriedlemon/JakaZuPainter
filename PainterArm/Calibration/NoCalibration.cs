namespace PainterArm.Calibration
{
    internal class NoCalibration : AbstractCalibrationBehavior
    {
        public NoCalibration(in JakaPainter painterArm) : base(painterArm) { }

        public override CoordinateSystem2D Calibrate()
        {
            throw new NotImplementedException("No calibration has been set");
        }
    }
}
