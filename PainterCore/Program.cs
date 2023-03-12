using PainterArm;

namespace PainterCore
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PaintingController paintingController = new(); 
            paintingController.Start();
            
/*
            JakaPainter painter = new("192.168.1.101");
            painter.MoveLinear(new CartesianPosition(0, 0, 30, 0, 0, 0), 10, 5, MovementType.Relative);
            //painter.WaitComplete();
            painter.MoveLinear(new CartesianPosition(0, 0, -30, 0, 0, 0), 10, 5, MovementType.Relative);*/
            //painter.WaitComplete();
        }
    }
}
