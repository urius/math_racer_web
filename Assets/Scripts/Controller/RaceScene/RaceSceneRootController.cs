using Data;
using Holders;
using Infra.Instance;
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

        public override void Initialize()
        {
            InitModel();
            InitView();
            InitControllers();
        }

        private void InitModel()
        {
            var complexityData = _complexityDataProvider.GetComplexityData(15, 10);
            //todo: use real data

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