using JakaAPI.Types.Math;

namespace PainterArm
{
    public class BrushesControl
    {
        private int _brushesAmmount = -1;

        private Dictionary<int, CartesianPosition> _brushesLocations;
        private CartesianPosition _washerLocations;
        private CartesianPosition _dryerLocations;

        public BrushesControl()
        {
            _brushesLocations = new Dictionary<int, CartesianPosition>();       
        }

        public CartesianPosition GetAvaliableBrushCoordinates()
        {
            return _brushesLocations[0];
        }   
    }
}
