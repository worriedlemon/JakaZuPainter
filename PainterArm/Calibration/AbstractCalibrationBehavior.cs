namespace PainterArm.Calibration
{
    /// <summary>
    /// Abstract class for managing canvas surface calibration
    /// </summary>
    public abstract class AbstractCalibrationBehavior
    {
        /// <summary>
        /// Painter Arm, which should be calibrated
        /// </summary>
        public JakaPainter PainterArm { get; private set; }

        public AbstractCalibrationBehavior(JakaPainter painter)
        {
            PainterArm = painter;
        }

        /// <summary>
        /// Function for calibrating surface
        /// </summary>
        /// <returns>Calibrated surface as <see cref="CoordinateSystem2D"/></returns>
        public abstract CoordinateSystem2D CalibrateSurface();
    }
}
