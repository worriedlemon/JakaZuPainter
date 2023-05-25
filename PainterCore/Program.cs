using JakaAPI.Types.Math;
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
            //PaintingController paintingController = new(); 
            //paintingController.Start();

            Vector3 axisX = new Vector3(2, 1, 1).Normalized();
            Vector3 axisY = new Vector3(1, -1, -1).Normalized();
            Vector3 axisZ = Vector3.VectorProduct(axisX, axisY).Normalized();
            Console.WriteLine("axisZ: " + axisZ);

            Matrix canvas = new Matrix(axisX, axisY, axisZ);
            Console.WriteLine("canvas:\n" + canvas);

            RPYRotation rpy = canvas.ToRPY();

            Console.WriteLine("rpy:\n" + rpy);

        }
    }
}
