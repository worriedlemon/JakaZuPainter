using JakaAPI.Types.Math;

namespace PainterCore
{
    // Temporary dummy until Robot Mixer addition
    public class RobotMixerDummy
    {
        public RobotMixerDummy()
        {

        }

        public void MixColor(CartesianPosition coordinates, ColorRGB color)
        {
            Console.WriteLine("Mixing... " + color);
            Thread.Sleep(5000);
            Console.WriteLine("Color mixed and added!");
        }
    }
}
