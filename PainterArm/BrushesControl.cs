using JakaAPI.Types;

namespace PainterArm
{
    public class BrushesControl
    {
        private int _brushesAmmount;
        private Dictionary<int, CartesianPosition> _brushesLocations;

        private CartesianPosition _dryer;
        private CartesianPosition _washer;

        public BrushesControl(int brushesAmmount)
        {
            _brushesAmmount = brushesAmmount;

            _brushesLocations = new Dictionary<int, CartesianPosition>();
        }

        // Addsnew brushes loacations to _brushesLocations, adds 1 to _brushesAmmount
        public void CalibrateBrushes()
        {
            _dryer = new CartesianPosition();
            _washer = new CartesianPosition();
        }

        public async Task<int> GetAvaliableBrushAsync()
        {
            return 0;
        }
    }
}
