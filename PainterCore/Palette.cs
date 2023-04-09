using JakaAPI.Types.Math;
using PainterArm;
using PainterArm.Calibration;

namespace PainterCore
{
    public class ColorRGB
    {
        public double Red
        {
            get
            {
                return _red;
            }
            set
            {
                _red = Bound(value);
            }
        }
        public double Green
        {
            get
            {
                return _green;
            }
            set
            {
                _green = Bound(value);
            }
        }
        public double Blue
        {
            get
            {
                return _blue;
            }
            set
            {
                _blue = Bound(value);
            }
        }

        private double _red;
        private double _green;
        private double _blue;

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
        private const int _safeZoneBorder = 10;
        private const int _colorOffset = 20;
        private int _currentX = _safeZoneBorder;
        private int _currentY = _safeZoneBorder;

        private Dictionary<ColorRGB, CartesianPosition> _colorsLocations;
        private Dictionary<ColorRGB, int> _strokesRemaining;

        private const int _strokesCountPerMixing = 5;

        private JakaPainter _painter;
        private CoordinateSystem2D? _coordinateSystem;
        public AbstractCalibrationBehavior CalibrationBehavior;

        public Palette(JakaPainter painterArm)
        {
            _colorsLocations = new Dictionary<ColorRGB, CartesianPosition>();
            _strokesRemaining = new Dictionary<ColorRGB, int>();
            _painter = painterArm;
            CalibrationBehavior = new ManualThreePointCalibration(_painter);
        }

        // Calibration function, set Palette to PainterArm coordinated + Gives it allowed borders for color adding
        public void CalibratePalette(CoordinateSystem2D paletteCoordinates) => _coordinateSystem = paletteCoordinates;

        public bool IsColorAdded(ColorRGB color) => _colorsLocations.ContainsKey(color);

        public CartesianPosition GetColorCoordinates(ColorRGB color) => _colorsLocations[color];

        // Add color to palette
        public void AddNewColor(ColorRGB color)
        {
            if (!_colorsLocations.ContainsKey(color))
            {
                _colorsLocations.Add(color, GetAvaliableLocation());
                _strokesRemaining.Add(color, _strokesCountPerMixing);
            }
        }

        public void UpdateColor(ColorRGB color)
        {
            if (_colorsLocations.ContainsKey(color))
            {
                _strokesRemaining[color] = _strokesCountPerMixing;
            }
        }

        // Substract this stroke from left strokes.
        public void SubstractStroke(ColorRGB color) => _strokesRemaining[color]--;

        // Get remaining strokes count for this color. 0 strokes mean Robot Mixer request
        public int GetStrokesLeft(ColorRGB color) => _strokesRemaining[color];

        // Calculate new coordinates on palette to place new color. Not done yet
        private CartesianPosition GetAvaliableLocation()
        {
            return new CartesianPosition(_coordinateSystem.Zero, _coordinateSystem.RPYParameters);

            if (_coordinateSystem == null) throw new InvalidOperationException("Pallete is not calibrated yet");

            Point point = _coordinateSystem.CanvasPointToWorldPoint(_currentX, _currentY);
            CartesianPosition avaliable = new CartesianPosition(point, _coordinateSystem.RPYParameters);

            if (_currentX + _colorOffset <= _coordinateSystem.MaxX - _safeZoneBorder)
            {
                _currentX += _colorOffset;
            }
            else if (_currentY + _colorOffset <= _coordinateSystem.MaxY - _safeZoneBorder)
            {
                _currentX = _safeZoneBorder;
                _currentY += _colorOffset;
            }
            else
            {
                throw new ArgumentException("Palette space ended!");
            }

            return avaliable;
        }
    }
}
