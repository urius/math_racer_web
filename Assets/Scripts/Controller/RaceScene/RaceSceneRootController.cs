using Data;
using Holders;
using Infra.Instance;
using Model;
using Model.RaceScene;
using UnityEngine;
using View.Gameplay.Race;

namespace Controller.RaceScene
{
    public class RaceSceneRootController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IComplexityDataProvider _complexityDataProvider = Instance.Get<IComplexityDataProvider>();
        
        private RaceContextView _contextView;
        private PlayerModel _playerModel;

        public override void Initialize()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            
            InitModel();
            InitView();
            InitControllers();
        }

        private void InitModel()
        {
            var complexityData = _complexityDataProvider.GetComplexityData(_playerModel.Level, _playerModel.ComplexityLevel);
            var raceModel = new RaceModel(CarKey.Bug, complexityData);
            _modelsHolder.SetRaceModel(raceModel);
        }

        private void InitView()
        {
            _contextView = Object.FindObjectOfType<RaceContextView>();
        }

        private void InitControllers()
        {
            var raceModel = _modelsHolder.GetRaceModel();
            
            InitChildController(new RaceSceneBackgroundController(_contextView.BgContainerView, _contextView.RoadContainerView));
            InitChildController(new RaceScenePlayerCarController(raceModel.PlayerCar, _contextView.PlayerCarTargetTransform));
            
            InitChildController(new RaceSceneTopPanelController(_contextView.RootCanvasView.TopPanelCanvasView));
            InitChildController(new RaceSceneQuestionsController(_contextView.RootCanvasView.RightPanelView));
        }

        public override void DisposeInternal()
        {
            _contextView = null;
        }
    }
}