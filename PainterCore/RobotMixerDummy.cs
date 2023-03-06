namespace PainterCore
{
    public class RobotMixerDummy
    {
        // Temporary dummy until Robot Mixer addition
        public async Task MixColor(ColorRGB color)
        {
            Console.WriteLine("Mixing... " + color);
            await Task.Delay(5000);
            Console.WriteLine("Color mixed and added!");
        }
    }
}
