using JakaAPI.Types;
using PainterArm;

namespace PainterCore
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            PaintingController paintingController = new(); 
            await paintingController.Start();
        }
    }
}
