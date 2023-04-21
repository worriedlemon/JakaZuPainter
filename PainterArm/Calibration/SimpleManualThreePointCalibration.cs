using JakaAPI.Types.Math;

namespace PainterArm.Calibration
{
    /// <summary>
    /// Manual surface calibration by three points
    /// </summary>
    public class SimpleManualThreePointCalibration : AbstractCalibrationBehavior
    {
        /// <summary>
        /// Constructor for initializing manual surface calibration
        /// </summary>
        /// <param name="painterArm"></param>
        public SimpleManualThreePointCalibration(in JakaPainter painterArm) : base(painterArm) { }

        public override CoordinateSystem2D Calibrate()
        {
            byte complete = 0;
            Point zero = new(), axisX = new(), axisY = new();
            RPYMatrix canvasRPY = new(180, 0, 0);

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
                            zero = PainterArm.GetRobotData().ArmCartesianPosition.Point;
                            canvasRPY = PainterArm.GetRobotData().ArmCartesianPosition.Rpymatrix;
                            complete |= 1;
                            break;
                        case 2:
                            axisX = PainterArm.GetRobotData().ArmCartesianPosition.Point;
                            complete |= 2;
                            break;
                        case 3:
                            axisY = PainterArm.GetRobotData().ArmCartesianPosition.Point;
                            complete |= 4;
                            break;
                        case 0:
                            if (complete != 7)
                            {
                                Console.WriteLine("Calibration is not complete. Set missing points");
                                break;
                            }

                            return new(zero, axisX, axisY, canvasRPY);
                        default:
                            throw new FormatException();
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("\nUnrecognized option. Try again.");
                }
            }
        }
    }
}
