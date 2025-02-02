using Data;

namespace Model.RaceScene
{
    public class CarRaceData
    {
        public readonly int Id;
        public readonly int CarPositionIndex;
        
        private readonly CarSettings _carSettings;

        public CarRaceData(CarSettings carSettings, int carPositionIndex, int id)
        {
            _carSettings = carSettings;
            CarPositionIndex = carPositionIndex;
            Id = id;
        }
        
        public CarKey CarKey => _carSettings.CarKey;
        public int Acceleration => _carSettings.Acceleration;
        public int MaxSpeed => _carSettings.MaxSpeed;
    }
}