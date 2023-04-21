namespace PainterArm.Calibration
{
    public class AutomaticThreePointCalibration : AbstractCalibrationBehavior
    {
        public AutomaticThreePointCalibration(JakaPainter painterArm) : base(painterArm) { }

        public override CoordinateSystem2D Calibrate()
        {
            throw new NotImplementedException("Automatic calibration not implemented yet. Use Manual Calibration behaviors instead for now.");
        }
    }
}
