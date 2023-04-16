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

            PaintingController paintingController = new(); 
            paintingController.Start();
        }
    }
}
