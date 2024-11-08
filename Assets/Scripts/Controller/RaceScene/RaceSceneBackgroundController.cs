using Data;
using Holders;
using Infra.Instance;
using Model.RaceScene;
using View.Gameplay.Race;

namespace Controller.RaceScene
{
    public class RaceSceneBackgroundController : ControllerBase
    {
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        
        private readonly BgContainerView _bgContainerView;
        private readonly RoadContainerView _roadContainerView;
        
        private CarModel _playerCarModel;

        public RaceSceneBackgroundController(BgContainerView bgContainerView, RoadContainerView roadContainerView)
        {
            _bgContainerView = bgContainerView;
            _roadContainerView = roadContainerView;
        }

        public override void Initialize()
        {
            _playerCarModel = _modelsHolder.GetRaceModel().PlayerCar;
            
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
        }

        private void OnGameplayFixedUpdate()
        {
            var distancePassed = _playerCarModel.CurrentSpeedKmph * Constants.KmphToMetersPerFrame;
            
            _bgContainerView.Move(distancePassed * 0.2f);
            _roadContainerView.Move(distancePassed);
        }
    }
}