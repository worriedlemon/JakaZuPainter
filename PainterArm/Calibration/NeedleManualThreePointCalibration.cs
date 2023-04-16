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
        private const double _needleLength = 20;

        /// <summary>
        /// Constructor for initializing manual surface calibration
        /// </summary>
        /// <param name="painterArm"></param>
        public NeedleManualThreePointCalibration(in JakaPainter painterArm) : base(painterArm) { }

        public override CoordinateSystem2D Calibrate()
        {
            byte complete = 0;
            Point zero = new(), axisX = new(), axisY = new();
            CoordinateSystem2D calibratedCoordinateSystem;

            Console.WriteLine("(1) Set zero pivot point\n" +
                "(2) Set X-axis point\n" +
                "(3) Set Y-axis point\n" +
                "(0) End calibration");

            while (true)
            {
                int option = Int32.Parse(Console.ReadLine()!);
                switch (option)
                {
                    case 1:
                        zero = GetVectorByNeedle(PainterArm.GetRobotData().ArmCartesianPosition);
                        complete |= 1;
                        break;
                    case 2:
                        axisX = GetVectorByNeedle(PainterArm.GetRobotData().ArmCartesianPosition);
                        complete |= 2;
                        break;
                    case 3:
                        axisY = GetVectorByNeedle(PainterArm.GetRobotData().ArmCartesianPosition);
                        complete |= 4;
                        break;
                    case 0:
                        if (complete != 7)
                        {
                            Console.WriteLine("Calibration is not complete. Set missing points");
                            break;
                        }

                        RPYMatrix rpyMatrix = GetRpyByPoints(zero, axisX, axisY);

                        calibratedCoordinateSystem = new(zero, axisX, axisY, rpyMatrix);

                        Console.WriteLine($"Calibrated coordinates:\n{calibratedCoordinateSystem}");

                        return calibratedCoordinateSystem;
                }
            }
        }

        private Point GetVectorByNeedle(CartesianPosition pos)
        {
            return (Point)((Vector3)pos.Point + new Vector3(0, 0, 1).RotateXYZ(pos.Rpymatrix.Rx, pos.Rpymatrix.Ry, pos.Rpymatrix.Rz) * _needleLength);
        }

        private RPYMatrix GetRpyByPoints(Point zero, Point axisX, Point axisY)
        {
            Vector3 vAxisX = (Vector3)axisX - (Vector3)zero;
            Vector3 vAxisY = (Vector3)axisY - (Vector3)zero;
            Vector3 vAxisZ = Vector3.VectorProduct(vAxisX, vAxisY);

            Vector3 vHeadToZero = (Vector3)PainterArm.GetRobotData().ArmCartesianPosition.Point - (Vector3)zero;
            double dotProduct = Vector3.DotProduct(vHeadToZero, vAxisZ);

            if (dotProduct > 0)
            {
                vAxisZ *= -1;
            }

            double rx = 180 / Math.PI * Math.Atan2(vAxisZ.Dy, vAxisZ.Dz);

            double ry = 180 / Math.PI * Math.Atan2(-vAxisZ.Dx, Math.Sqrt(vAxisZ.Dy * vAxisZ.Dy + vAxisZ.Dz * vAxisZ.Dz));

            double rz = 180 / Math.PI * Math.Atan2(vAxisY.Dx, vAxisX.Dx);

            return new RPYMatrix(rx, ry, rz);
        }
    }
}
