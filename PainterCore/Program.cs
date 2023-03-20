using PainterArm;

namespace PainterCore
{
    internal class Program
    {
        private void PrintLicenseInfo()
        {
            Console.WriteLine("\tRobot-painter  Copyright (C) 2023  E. Shteinberg, A. Skuratov\n" +
                "\tThis program comes with ABSOLUTELY NO WARRANTY.\n" +
                "\tThis is free software, and you are welcome to redistribute it under certain conditions.\n")
        }

        static void Main(string[] args)
        {
            PrintLicenseInfo();

            PaintingController paintingController = new(); 
            paintingController.Start();

            /*JakaPainter painter = new("192.168.1.101");
            painter.MoveLinear(new CartesianPosition(0, 0, 30, 0, 0, 0), 10, 5, MovementType.Relative);
            //painter.WaitComplete();
            painter.MoveLinear(new CartesianPosition(0, 0, -30, 0, 0, 0), 10, 5, MovementType.Relative);*/
            //painter.WaitComplete();
        }
    }
}
