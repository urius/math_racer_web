using System.Linq;
using Controller.Common;
using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.RaceScene;
using Providers;
using Services;
using UnityEngine;
using View.Gameplay.Race;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Controller.RaceScene
{
    public class RaceSceneRootController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IComplexityDataProvider _complexityDataProvider = Instance.Get<IComplexityDataProvider>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ICarDataProvider _carDataProvider = Instance.Get<ICarDataProvider>();
        private readonly IP2PRoomService _p2pRoomService = Instance.Get<IP2PRoomService>();
        private readonly Random _random = new();
        
        private RaceContextView _contextView;
        private PlayerModel _playerModel;
        private RaceModel _raceModel;
        private SessionDataModel _sessionDataModel;

        private bool IsMultiplayerGame =>
            (_sessionDataModel.RequestSceneParams as RequestRaceSceneParams)?.IsMultiplayer ?? false;

        private bool IsSinglePlayerGame => !IsMultiplayerGame;

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
            _eventBus.Subscribe<RequestSettingsPopupEvent>(OnRequestSettingsPopupEvent);
        }

        private void Unsubscribe()
        {
            _updatesProvider.GameplayUpdate += OnGameplayUpdate;
            _eventBus.Unsubscribe<RequestSettingsPopupEvent>(OnRequestSettingsPopupEvent);
        }

        private void InitModel()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            _sessionDataModel = _modelsHolder.GetSessionDataModel();
            
            if (IsSinglePlayerGame)
            {
                var complexityData = _complexityDataProvider.GetComplexityData(_playerModel.Level, _playerModel.ComplexityLevel);
                var unlockedCars = _carDataProvider.GetUnlockedCarsByLevel(_playerModel.Level);
                var opponentCarKey = unlockedCars[_random.Next(unlockedCars.Count)].CarKey;

                _raceModel = new RaceModel(
                    new CarRaceModelData(_playerModel.CurrentCar, carPositionIndex: 0),
                    complexityData,
                    new CarRaceModelData(opponentCarKey, carPositionIndex: 1));
            }
            else
            {
                var complexityData =
                    _complexityDataProvider.GetComplexityData(_playerModel.Level, _playerModel.ComplexityLevel);
                
                var netPlayersData = _p2pRoomService.PlayersData;
                var playerCarData = ToCarRaceModelData(netPlayersData.SelfData);
                var opponentCarDataList = netPlayersData.PlayerDataByConnection.Values
                    .Select(ToCarRaceModelData)
                    .ToArray();

                _raceModel = new RaceModel(
                    playerCarData,
                    complexityData,
                    opponentCarDataList[0],
                    opponentCarDataList.Length > 0 ? opponentCarDataList[1] : null);

            }

            _modelsHolder.SetRaceModel(_raceModel);
        }

        private CarRaceModelData ToCarRaceModelData(P2PPlayerData p2pPlayerData)
        {
            return new CarRaceModelData(p2pPlayerData.CarKey, p2pPlayerData.PositionIndex);
        }

        private void InitView()
        {
            _contextView = Object.FindObjectOfType<RaceContextView>();
        }

        private void InitControllers()
        {
            if (IsSinglePlayerGame)
            {
                InitChildController(new SingleRacePlayerCarController(_raceModel.PlayerCar,
                    _contextView.CarContainerTransforms[_raceModel.PlayerCar.PositionIndex]));
                
                foreach (var opponentCarModel in _raceModel.OpponentCarModels)
                {
                    InitChildController(new RaceBotCarController(opponentCarModel,
                        _contextView.CarContainerTransforms[opponentCarModel.PositionIndex]));
                }
            }
            else
            {
                
            }

            InitChildController(new RaceStartLineController(_contextView.StartLineTransform, _contextView.TrafficLightView));
            InitChildController(new RaceBackgroundController(
                _contextView.BgContainerView,
                _contextView.RoadContainerView));
            InitChildController(new RaceFinishController(
                _contextView.FinishLineTransform,
                _contextView.PlayerCarTargetTransform,
                _contextView.RootCanvasView));
            
            InitChildController(new RaceTopPanelController(_contextView.RootCanvasView.TopPanelCanvasView));
            InitChildController(new RaceQuestionsController(_contextView.RootCanvasView.RightPanelView));
        }

        private void OnRequestSettingsPopupEvent(RequestSettingsPopupEvent e)
        {
            InitChildController(
                new SettingsPopupController(_contextView.RootCanvasView.PopupsCanvasTransform, isShortVersion: false));
        }

        private void OnGameplayUpdate()
        {
            _raceModel.Update(Time.deltaTime);
        }
    }
}