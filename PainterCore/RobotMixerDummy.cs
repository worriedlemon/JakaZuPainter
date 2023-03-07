using JakaAPI.Types;

namespace PainterCore
{
    // Temporary dummy until Robot Mixer addition
    public class RobotMixerDummy
    {
        public RobotMixerDummy()
        {

        }

        public async Task MixColor(CartesianPosition coordinates, ColorRGB color)
        {
            Console.WriteLine("Mixing... " + color);
            await Task.Delay(5000);
            Console.WriteLine("Color mixed and added!");
        }
    }
}
