using Infra.Instance;
using Model.RaceScene;
using Providers;
using UnityEngine;
using View.UI.RaceScene;

namespace Controller.RaceScene
{
    public class RaceSceneTopPanelController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        
        public readonly UITopPanelCanvasView TopPanelCanvasView;
        
        private RaceModel _raceModel;
        private CarModel _playerCarModel;

        public RaceSceneTopPanelController(UITopPanelCanvasView topPanelCanvasView)
        {
            TopPanelCanvasView = topPanelCanvasView;
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
            _updatesProvider.GameplayFixedUpdate += OnGameplayFixedUpdate;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
        }

        private void OnGameplayFixedUpdate()
        {
            TopPanelCanvasView.SetText(
                $"speed: {Mathf.RoundToInt(_playerCarModel.CurrentSpeedKmph)} kmh\ndistance: {Mathf.RoundToInt(_playerCarModel.PassedMeters)} m");
        }
    }
}