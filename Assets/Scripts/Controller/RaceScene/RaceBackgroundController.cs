using Infra.Instance;
using Model.RaceScene;
using Providers;
using View.Gameplay.Race;

namespace Controller.RaceScene
{
    public class RaceBackgroundController : ControllerBase
    {
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        
        private readonly BgContainerView _bgContainerView;
        private readonly RoadContainerView _roadContainerView;

        private CarModel _playerCarModel;
        private RaceModel _raceModel;

        public RaceBackgroundController(BgContainerView bgContainerView, RoadContainerView roadContainerView)
        {
            _bgContainerView = bgContainerView;
            _roadContainerView = roadContainerView;
        }

        public override void Initialize()
        {
            _raceModel = _modelsHolder.GetRaceModel();
            _playerCarModel = _raceModel.PlayerCar;
            
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _updatesProvider.GameplayUpdate += OnGameplayUpdate;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayUpdate -= OnGameplayUpdate;
        }

        private void OnGameplayUpdate()
        {
            var distancePassed = _playerCarModel.CurrentUpdateMetersPassed;
            
            _bgContainerView.Move(distancePassed * 0.2f);
            _roadContainerView.Move(distancePassed);
        }
    }
}