using JakaAPI.Types.Math;

namespace PainterArm.Calibration
{
    /// <summary>
    /// Manual surface calibration by three points using needle tip
    /// </summary>
    public class NeedleManualThreePointCalibration : AbstractCalibrationBehavior
    {
        /// <summary>
        /// Length between the end of a calibration needle and the painter arm
        /// </summary>
        private readonly double _needleLength;

        /// <summary>
        /// Constructor for initializing manual surface calibration
        /// </summary>
        /// <param name="painterArm"></param>
        /// <param name="needleLength">Length of the used needle</param>
        public NeedleManualThreePointCalibration(in JakaPainter painterArm, double needleLength) : base(painterArm)
        {
            _needleLength = needleLength;
        }

        public override CoordinateSystem2D Calibrate()
        {
            byte complete = 0;
            Point zero = new(), axisX = new(), axisY = new();

            Console.WriteLine("(1) Set zero pivot point\n" +
                "(2) Set X-axis point\n" +
                "(3) Set Y-axis point\n" +
                "(0) End calibration");

            while (true)
            {
                Console.Write("> ");
                try
                {
                    int option = Int32.Parse(Console.ReadLine()!);
                    switch (option)
                    {
                        case 1:
                            zero = GetCanvasPointByNeedle(PainterArm.GetRobotData().ArmCartesianPosition);
                            complete |= 1;
                            break;
                        case 2:
                            axisX = GetCanvasPointByNeedle(PainterArm.GetRobotData().ArmCartesianPosition);
                            complete |= 2;
                            break;
                        case 3:
                            axisY = GetCanvasPointByNeedle(PainterArm.GetRobotData().ArmCartesianPosition);
                            complete |= 4;
                            break;
                        case 0:
                            if (complete != 7)
                            {
                                Console.WriteLine("Calibration is not complete. Set missing points");
                                break;
                            }

                            return new(zero, axisX, axisY, GetRPYByPoints(zero, axisX, axisY, true));
                        default:
                            throw new FormatException();
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Unrecocognized value. Try again.");
                }
            }
        }

        private Point GetCanvasPointByNeedle(CartesianPosition pos)
        {
            Console.WriteLine(pos);
            Matrix rm = Matrix.RotationMatrix(pos.Rpymatrix.Rx, pos.Rpymatrix.Ry, pos.Rpymatrix.Rz);
            Point p = (Point)((Vector3)pos.Point + rm * new Vector3(0, 0, _needleLength));
            Console.WriteLine(p);
            return p;
        }

        private RPYRotation GetRPYByPoints(Point zero, Point axisX, Point axisY, bool inversed = false)
        {
            Vector3 vAxisX = ((Vector3)axisX - (Vector3)zero).Normalized();
            Vector3 vAxisY = ((Vector3)axisY - (Vector3)zero).Normalized();
            Vector3 vAxisZ = CoordinateSystem2D.FixZShiftByPoint(Vector3.VectorProduct(vAxisX, vAxisY).Normalized(), zero, PainterArm.GetRobotData().ArmCartesianPosition.Point);

            Console.WriteLine($"AxisX: {vAxisX}\nAxisY: {vAxisY}\nAxisY: {vAxisZ}");

            return (new Matrix(vAxisX, vAxisY, vAxisZ) * (inversed ? -1 : 1)).ToRPY();
        }
    }
}
