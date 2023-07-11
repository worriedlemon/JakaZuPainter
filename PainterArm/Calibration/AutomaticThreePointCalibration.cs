using JakaAPI.Types;
using JakaAPI.Types.Math;
using PainterArm.MathExtensions;

namespace PainterArm.Calibration
{
    public class AutomaticThreePointCalibration : AbstractCalibrationBehavior
    {
        public AutomaticThreePointCalibration(JakaPainter painterArm) : base(painterArm) { }

        // Калибровка через 3д сенсор. Его следует установить на расстоянии _initialOffset от предполагаемых нулевой, X и Y точкой холста.

        // Длина от головы робота до конца сенсора
        private double _sensorLength = 95;

        // Расстояние до предполагаемой точки
        private double _maxLength = 10;

        // Поэтапное дивжение сенсора
        private double _smoothness = 0.2;

        // Аналоговый вход, к которому подключен сенсор
        private int _AIindex = 1;

        private double _speed = 10;
        private double _acceleration = 5;


        public override CoordinateSystem2D Calibrate()
        {
            //throw new Exception();
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
                    Thread.Sleep(10000);
                    switch (option)
                    {
                        case 1:
                            zero = GetCanvasPointBySensor();
                            complete |= 1;
                            break;
                        case 2:
                            axisX = GetCanvasPointBySensor();
                            complete |= 2;
                            break;
                        case 3:
                            axisY = GetCanvasPointBySensor();
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

        private Point GetCanvasPointBySensor()
        {
            CartesianPosition initialPos = PainterArm.GetRobotData().ArmCartesianPosition;

            Matrix rm = Matrix.RotationMatrix(initialPos.Rpymatrix.Rx, initialPos.Rpymatrix.Ry, initialPos.Rpymatrix.Rz);

            Vector3 start = (Vector3)initialPos.Point;

            Vector3 target = start + rm * new Vector3(0, 0, _maxLength);

            Vector3 direction = (target - start);
            Vector3 unitSmoothDirection = direction.Normalized() * _smoothness;

            Vector3 currVect = start;
            Vector3 distancePassed = new Vector3(0, 0, 0);

            while (distancePassed.Length() < _maxLength)
            {
                distancePassed += unitSmoothDirection;
                currVect += unitSmoothDirection;

                PainterArm.MoveLinear(new CartesianPosition((Point)currVect, initialPos.Rpymatrix), _speed, _acceleration, MovementType.Absolute);

                double state = PainterArm.GetRobotData().AIStatus[_AIindex];
                Console.WriteLine("state: " + state);

                if (state >= 45 && state <= 50)
                {
                    continue;
                }
                else // state = 0 - Коллизия
                {
                    return PointBySensor(currVect, rm);
                }
            }

            return PointBySensor(target, rm);
        }

        private Point PointBySensor(Vector3 curr, Matrix rm)
        {
            return (Point)(curr + rm * new Vector3(0, 0, _sensorLength));
        }

        private RPYRotation GetRPYByPoints(Point zero, Point axisX, Point axisY, bool inversed = false)
        {
            Vector3 vAxisX = ((Vector3)axisX - (Vector3)zero).Normalized();
            Vector3 vAxisY = ((Vector3)axisY - (Vector3)zero).Normalized();
            Vector3 vAxisZ = CoordinateSystem2D.FixZShiftByPoint(Vector3.VectorProduct(vAxisX, vAxisY).Normalized(), zero, PainterArm.GetRobotData().ArmCartesianPosition.Point);

            Console.WriteLine($"AxisX: {vAxisX}\nAxisY: {vAxisY}\nAxisY: {vAxisZ}");

            return (vAxisZ * (inversed ? -1 : 1)).ToRPY().MainSolution;
        }
    }
}
