using JakaAPI.Types.Math;
using PainterArm;
using PainterArm.Calibration;
using PainterArm.MathExtensions;

namespace PainterCore
{
    public struct ColorPickData
    {
        public ColorPickData(double pickRadius, bool randomPick, double pressOffset)
        {
            PickRadius = pickRadius;
            RadndomPick = randomPick;
            PressOffset = pressOffset;
        }

        public double PickRadius { get; private set; }

        public bool RadndomPick { get; private set; }

        public double PressOffset { get; private set; }
    }

    internal struct ColorCupMathModel
    {
        public ColorCupMathModel(double radius, double height, double wallThickness, double bottomThickness)
        {
            Radius = radius;
            Height = height;
            WallThickness = wallThickness;
            BottomThickness = bottomThickness;
        }

        public double Radius { get; private set; }

        public double Height { get; private set; }

        public double WallThickness { get; private set; }

        public double BottomThickness { get; private set; }
    }

    public class ColorRGB
    {
        public double Red { get; private set; }
        public double Green { get; private set; }
        public double Blue { get; private set; }

        public ColorRGB(double red, double green, double blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public override string ToString()
        {
            return $"[{Red},{Green},{Blue}]";
        }
    }

    public class Palette
    {
        private ColorCupMathModel _cup = new ColorCupMathModel(30, 12, 1, 1);

        private Dictionary<ColorRGB, Point> _colorsLocations;
        private Dictionary<ColorRGB, int> _strokesRemaining;

        private const int _strokesCountPerMixing = 5;

        private JakaPainter _painter;
        public CoordinateSystem2D? _coordinateSystem { get; private set; }
        public AbstractCalibrationBehavior CalibrationBehavior;

        public Palette(JakaPainter painterArm)
        {
            _painter = painterArm;
            _colorsLocations = new Dictionary<ColorRGB, Point>();
            _strokesRemaining = new Dictionary<ColorRGB, int>();
            CalibrationBehavior = new NeedleManualThreePointCalibration(_painter);
        }

        // Calibration function, set Palette to PainterArm coordinated + Gives it allowed borders for color adding
        public void CalibratePalette(CoordinateSystem2D paletteCoordinates) => _coordinateSystem = paletteCoordinates;

        public bool IsColorAdded(ColorRGB color) => _colorsLocations.ContainsKey(color);

        public Point GetColorPoint(ColorRGB color) => _colorsLocations[color];

        // Add color to palette
        public void AddNewColor(ColorRGB color)
        {
            if (!_colorsLocations.ContainsKey(color))
            {
                _colorsLocations.Add(color, GetAvaliableLocation().Point);
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
        public CartesianPosition GetAvaliableLocation()
        {
            return new CartesianPosition(_coordinateSystem!.Zero, _coordinateSystem.RPYParameters);
        }

        private Point GetPointInRadius(Point zero, double radius, double angleRadians)
        {
            return new Point(
                zero.X + radius * Math.Cos(angleRadians),
                zero.Y + radius * Math.Sin(angleRadians));
        }

        // Возвращает точки в локальной! системе координат
        public List<Point> PickColorInstruction(ColorRGB color, ColorPickData data)
        {
            List<Point> points = new List<Point>();

            Point color_point = new Point(10, 10, 0); // Заменить на коорды цвета

            Point start_point_raw = GetPointInRadius(color_point, data.PickRadius, 0);

            Point start_point_air = start_point_raw + new Vector3(0, 0, _cup.BottomThickness + _cup.Height + 30);
            points.Add(start_point_air);

            Point start_point = start_point_raw + new Vector3(0, 0, _cup.BottomThickness);
            points.Add(start_point);

            Point pick_point = color_point + new Vector3(0, 0, _cup.BottomThickness - data.PressOffset);
            points.Add(pick_point);

            Point up_pick_point = pick_point + new Vector3(0, 0, _cup.BottomThickness + _cup.Height * 1);
            points.Add(up_pick_point);

            Point outer_point = GetPointInRadius(color_point, _cup.Radius + _cup.WallThickness + 10, 0);
            points.Add(outer_point);

            Point inner_point = GetPointInRadius(color_point, _cup.Radius - _cup.WallThickness - 10, 0);
            points.Add(inner_point);

            points.Add(start_point_air);

            return points;
        }
    }
}
