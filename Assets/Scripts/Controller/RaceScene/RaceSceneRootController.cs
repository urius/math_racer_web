using Data;
using Infra.Instance;
using Model;
using Model.RaceScene;
using Providers;
using UnityEngine;
using View.Gameplay.Race;

namespace Controller.RaceScene
{
    public class RaceSceneRootController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IComplexityDataProvider _complexityDataProvider = Instance.Get<IComplexityDataProvider>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        
        private RaceContextView _contextView;
        private PlayerModel _playerModel;
        private RaceModel _raceModel;

        public override void Initialize()
        {
            InitModel();
            InitView();
            InitControllers();

            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
            
            _contextView = null;
        }

        private void Subscribe()
        {
            _updatesProvider.GameplayUpdate += OnGameplayUpdate;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayUpdate += OnGameplayUpdate;
        }

        private void InitModel()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            
            var complexityData = _complexityDataProvider.GetComplexityData(_playerModel.Level, _playerModel.ComplexityLevel);
            _raceModel = new RaceModel(_playerModel.CurrentCar, CarKey.Bug, complexityData);
            
            _modelsHolder.SetRaceModel(_raceModel);
        }

        private void InitView()
        {
            _contextView = Object.FindObjectOfType<RaceContextView>();
        }

        private void InitControllers()
        {
            InitChildController(new RaceScenePlayerCarController(_raceModel.PlayerCar, _contextView.PlayerCarTargetTransform));
            InitChildController(new RaceSceneBotCarController(_raceModel.BotCar, _contextView.BotCarTargetTransform));
            InitChildController(new RaceSceneStartLineController(_contextView.StartLineTransform, _contextView.TrafficLightView));
            InitChildController(new RaceSceneBackgroundController(
                _contextView.BgContainerView,
                _contextView.RoadContainerView));
            InitChildController(new RaceFinishController(
                _contextView.FinishLineTransform,
                _contextView.PlayerCarTargetTransform,
                _contextView.RootCanvasView));
            
            InitChildController(new RaceSceneTopPanelController(_contextView.RootCanvasView.TopPanelCanvasView));
            InitChildController(new RaceSceneQuestionsController(_contextView.RootCanvasView.RightPanelView));
        }

        private void OnGameplayUpdate()
        {
            _raceModel.Update(Time.deltaTime);
        }
    }
}