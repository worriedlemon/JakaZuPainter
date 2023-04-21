namespace PainterArm.Calibration
{
    internal class NoCalibration : AbstractCalibrationBehavior
    {
        public NoCalibration(in JakaPainter painterArm) : base(painterArm) { }

        public override ICalibratable Calibrate()
        {
            throw new NotImplementedException("No calibration has been set");
        }
    }
}
