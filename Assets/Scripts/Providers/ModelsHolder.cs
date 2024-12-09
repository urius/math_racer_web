using Model;
using Model.RaceScene;

namespace Providers
{
    public class ModelsHolder : IModelsHolder
    {
        private RaceModel _raceModel;
        private PlayerModel _playerModel;
        
        public void SetRaceModel(RaceModel raceModel)
        {
            _raceModel = raceModel;
        }
        
        public RaceModel GetRaceModel()
        {
            return _raceModel;
        }
        
        public void SetPlayerModel(PlayerModel playerModel)
        {
            _playerModel = playerModel;
        }
        
        public PlayerModel GetPlayerModel()
        {
            return _playerModel;
        }
    }

    public interface IModelsHolder
    {
        public void SetRaceModel(RaceModel raceModel);
        public RaceModel GetRaceModel();
        
        public void SetPlayerModel(PlayerModel playerModel);
        public PlayerModel GetPlayerModel();
    }
}