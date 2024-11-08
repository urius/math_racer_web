using Model.RaceScene;

namespace Holders
{
    public class ModelsHolder : IModelsHolder
    {
        private RaceModel _raceModel;
        
        public void SetRaceModel(RaceModel raceModel)
        {
            _raceModel = raceModel;
        }
        
        public RaceModel GetRaceModel()
        {
            return _raceModel;
        }
    }

    public interface IModelsHolder
    {
        public void SetRaceModel(RaceModel raceModel);
        public RaceModel GetRaceModel();
    }
}