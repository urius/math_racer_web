using Data;
using Events;
using Extensions;
using Infra.EventBus;
using Infra.Instance;
using Model.RaceScene;
using Providers;
using Providers.LocalizationProvider;
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
        private readonly ICarDataProvider _carDataProvider = Instance.Get<ICarDataProvider>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();

        private readonly UITopPanelCanvasView _topPanelCanvasView;
        private readonly UIRaceSceneRaceSchemaView _raceSchemaView;
        
        private RaceModel _raceModel;
        private CarModel _playerCarModel;

        public RaceSceneTopPanelController(UITopPanelCanvasView topPanelCanvasView)
        {
            _topPanelCanvasView = topPanelCanvasView;
            _raceSchemaView = _topPanelCanvasView.RaceSchemaView;
        }

        public override void Initialize()
        {
            _raceModel = _modelsHolder.GetRaceModel();
            _playerCarModel = _raceModel.PlayerCar;

            SetupRaceSchemaView();
            
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void SetupRaceSchemaView()
        {
            _raceSchemaView.SetDistanceText($"{_raceModel.DistanceMeters}{_localizationProvider.GetLocale(LocalizationKeys.MetersShort)}");
            
            var playerCarData = _carDataProvider.GetCarData(_playerCarModel.CarKey);
            _raceSchemaView.SetPlayerCarIconSprite(playerCarData.IconSprite);
            _raceSchemaView.SetPlayerCarPassedDistancePercent(0);
            
            var botCarData = _carDataProvider.GetCarData(_raceModel.BotCar.CarKey);
            _raceSchemaView.SetBotCarIconSprite(botCarData.IconSprite);
            _raceSchemaView.SetBotCarPassedDistancePercent(0);
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
            _topPanelCanvasView.SetText($"{Mathf.RoundToInt(_playerCarModel.CurrentSpeedKmph)} {_localizationProvider.GetLocale(LocalizationKeys.KmH)}");

            UpdateRaceSchemaPositions();
        }

        private void UpdateRaceSchemaPositions()
        {
            var playerPassedDistancePercent = _playerCarModel.PassedMeters / _raceModel.DistanceMeters;
            _raceSchemaView.SetPlayerCarPassedDistancePercent(playerPassedDistancePercent);

            var botPassedDistancePercent = _raceModel.BotCar.PassedMeters / _raceModel.DistanceMeters;
            _raceSchemaView.SetBotCarPassedDistancePercent(botPassedDistancePercent);
        }
    }
}