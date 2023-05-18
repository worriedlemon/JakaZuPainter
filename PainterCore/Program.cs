using JakaAPI.Types.Math;

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
            PrintLicenseInfo();

            Matrix3x3 matrix3X3 = -1 * Matrix3x3.RotationMatrix(90, 45, -30);
            Console.WriteLine(matrix3X3);
            Console.WriteLine(matrix3X3.ToRPY());

            //PaintingController paintingController = new(); 
            //paintingController.Start();
        }
    }
}
