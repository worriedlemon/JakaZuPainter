using JakaAPI.Types;
using PainterArm;

namespace PainterCore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //PaintingController paintingController = new();
            //paintingController.Start();

            JakaPainter painter = new("192.168.1.100");
        }
    }
}
