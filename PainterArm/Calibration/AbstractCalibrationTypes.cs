namespace PainterArm.Calibration
{
    /// <summary>
    /// Interface for implementing a calibratable data structure
    /// </summary>
    public interface ICalibratable { }

    /// <summary>
    /// Abstract class for managing canvas surface calibration
    /// </summary>
    public abstract class AbstractCalibrationBehavior
    {
        /// <summary>
        /// Painter Arm, which should be calibrated
        /// </summary>
        public JakaPainter PainterArm { get; }

        public AbstractCalibrationBehavior(in JakaPainter painter)
        {
            PainterArm = painter;
        }

        /// <summary>
        /// Function for calibrating surface
        /// </summary>
        /// <returns>Calibrated surface as <see cref="ICalibratable"/></returns>
        public abstract ICalibratable Calibrate();
    }
}