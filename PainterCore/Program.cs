using JakaAPI.Types.Math;
using PainterArm;
using System.Numerics;
using Vector3 = JakaAPI.Types.Math.Vector3;

namespace PainterCore
{
    internal class Program
    {
        private static void PrintLicenseInfo()
        {
            Console.WriteLine("\tRobot-painter  Copyright (C) 2023  E. Shteinberg, A. Skuratov\n" +
                "\tThis program comes with ABSOLUTELY NO WARRANTY.\n" +
                "\tThis is free software, and you are welcome to redistribute it under certain conditions.\n");
        }

        static void Main(string[] args)
        {
            //PrintLicenseInfo();
            //PaintingController paintingController = new(); 
            //paintingController.Start();

            Point zero = new Point(-157.96745248213188, 154.31557687247212, 442.22113895878965);
            Point axisX = new Point(-19.549353131347225, 211.41148132461635, 440.802360127826);
            Point axisY = new Point(-169.96679380787995, 184.66657809226405, 559.126258323734);

            Vector3 vAxisX = ((Vector3)axisX - (Vector3)zero).Normalized();
            Vector3 vAxisY = ((Vector3)axisY - (Vector3)zero).Normalized();
            Vector3 vAxisZ = Vector3.VectorProduct(vAxisX, vAxisY).Normalized();
            Vector3 normale = new Vector3(-0.62, 0.3, 0.73);

            /*     Vector3 v1 = new Vector3();
                 Vector3 v2 = new Vector3();
                 Vector3 normale = new Vector3(-0.62, 0.3, 0.73);
     */
            RPYRotation rpy = ((-1 * new Matrix(vAxisX, vAxisY, vAxisZ))).ToRPY();

            /*Matrix F = new Matrix(new double[3, 3]
            {
                {-0.617749315031253, 0.29891095888609026, 0.7273499999561529},
                { 0.6594906278316254, -0.3191083683056252, 0.6806188074682993 },
                { 0.43556092810428065, 0.9001592514155135, 0 }
            });

            Matrix F1 = F.Rounded(4).ReverseMatrix();

            Console.WriteLine(F1);*/
        }
    }
}
