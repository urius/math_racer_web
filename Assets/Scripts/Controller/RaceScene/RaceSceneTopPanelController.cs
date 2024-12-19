using Events;
using Extensions;
using Infra.EventBus;
using Infra.Instance;
using Model.RaceScene;
using Providers;
using UnityEngine;
using Utils.AudioManager;
using View.UI.RaceScene;

namespace Controller.RaceScene
{
    public class RaceSceneTopPanelController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();

        private readonly UITopPanelCanvasView _topPanelCanvasView;
        
        private RaceModel _raceModel;
        private CarModel _playerCarModel;

        public RaceSceneTopPanelController(UITopPanelCanvasView topPanelCanvasView)
        {
            _topPanelCanvasView = topPanelCanvasView;
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
            _topPanelCanvasView.SettingsButtonClicked += OnSettingsButtonClicked;
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayFixedUpdate -= OnGameplayFixedUpdate;
            _topPanelCanvasView.SettingsButtonClicked -= OnSettingsButtonClicked;
        }

        private void OnSettingsButtonClicked()
        {
            _eventBus.Dispatch(new RequestSettingsPopupEvent());
            
            _audioPlayer.PlayButtonSound();
        }

        private void OnGameplayFixedUpdate()
        {
            _topPanelCanvasView.SetText(
                $"speed: {Mathf.RoundToInt(_playerCarModel.CurrentSpeedKmph)} kmh\ndistance: {Mathf.RoundToInt(_playerCarModel.PassedMeters)} m");
        }
    }
}