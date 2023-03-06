using JakaAPI.Types;

namespace PainterCore
{
    public class RobotMixerDummy
    {
        // Temporary dummy until Robot Mixer addition
        public async Task MixColor(Point coordinates, ColorRGB color)
        {
            Console.WriteLine("Mixing... " + color);
            await Task.Delay(5000);
            Console.WriteLine("Color mixed and added!");
        }
    }
}
