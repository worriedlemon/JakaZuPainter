using JakaAPI.Types;

namespace PainterCore
{
    public class ColorRGB
    {
        public double Red
        {
            get
            {
                return Red;
            }
            set
            {
                Red = Bound(value);
            }
        }
        public double Green
        {
            get
            {
                return Green;
            }
            set
            {
                Green = Bound(value);
            }
        }
        public double Blue
        {
            get
            {
                return Blue;
            }
            set
            {
                Blue = Bound(value);
            }
        }

        public ColorRGB(double red, double green, double blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        private static double Bound(double value)
        {
            return value < 0 ? 0 : (value > 255 ? 255 : value);
        }

        public override string ToString()
        {
            return $"[{Red},{Green},{Blue}]";
        }
    }

    public class Palette
    {
        private Dictionary<ColorRGB, CartesianPosition> _colorsLocations;
        private Dictionary<ColorRGB, int> _strokesRemaining;
        private bool _isCalibrated = false;
        private const int _strokesCountPerMixing = 5;

        public Palette()
        {
            _colorsLocations = new Dictionary<ColorRGB, CartesianPosition>();
            _strokesRemaining = new Dictionary<ColorRGB, int>();
        }

        // Calibration function, set Palette to PainterArm coordinated
        public void CalibratePalette()
        {
            _isCalibrated = true;
        }

        public bool IsColorAdded(ColorRGB color)
        {
            return _colorsLocations.ContainsKey(color);
        }

        public CartesianPosition GetColorCoordinates(ColorRGB color)
        {
            return _colorsLocations[color];
        }

        // Add color to palette
        public void AddColor(ColorRGB color)
        {
            if (!_colorsLocations.ContainsKey(color))
            {
                _colorsLocations.Add(color, GetAvaliableLocation());
                _strokesRemaining.Add(color, _strokesCountPerMixing);
            }
        }

        // Get single stroke from palette, substract this stroke from left strokes.
        public CartesianPosition TakeStrokeFromPallete(ColorRGB color)
        {
            _strokesRemaining[color]--;
            return _colorsLocations[color];
        }

        // Get remaining strokes count for this color. 0 strokes mean Robot Mixer request
        public int GetStrokesLeft(ColorRGB color)
        {
            return _strokesRemaining[color];
        }
    
        // Calculate new coordinates on palette to place new color. Not done yet
        private CartesianPosition GetAvaliableLocation()
        {
            return new CartesianPosition();
        }
    }
}
