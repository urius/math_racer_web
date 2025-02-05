using System.Collections.Generic;
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
        private IReadOnlyList<CarSettings> _unlockedCars;

        private bool IsMultiplayerGame =>
            (_sessionDataModel.RequestSceneParams as RequestRaceSceneParams)?.IsMultiplayer ?? false;

        private bool IsSinglePlayerGame => !IsMultiplayerGame;

        public override void Initialize()
        {
            InitModel();
            InitView();
            InitControllers();

            _raceModel.StartRace();

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
            _unlockedCars = _carDataProvider.GetUnlockedCarsByLevel(_playerModel.Level);
            
            if (IsSinglePlayerGame)
            {
                var complexityData = _complexityDataProvider.GetComplexityData(_playerModel.Level, _playerModel.ComplexityLevel);
                var opponentsCount =
                    1 + _random.Next(Mathf.Clamp((int)(_playerModel.Level * 0.5), 1, Constants.MaxOpponentsCount));

                var opponentsRaceData = Enumerable.Range(0, opponentsCount)
                    .Select(CreateOpponentRaceData)
                    .ToArray();

                _raceModel = new RaceModel(
                    GetRaceDistance(opponentsCount),
                    new CarRaceData(_carDataProvider.GetCarData(_playerModel.CurrentCar), carPositionIndex: 0, id: 1),
                    complexityData,
                    opponentsRaceData[0],
                    opponentsRaceData.Length > 1 ? opponentsRaceData[1] : null,
                    opponentsRaceData.Length > 2 ? opponentsRaceData[2] : null);
            }
            else
            {
                var complexityData =
                    _complexityDataProvider.GetComplexityData(_playerModel.Level, _playerModel.ComplexityLevel);
                
                var netPlayersData = _p2pRoomService.PlayersData;
                var playerCarData = ToCarRaceModelData(netPlayersData.LocalPlayerData);
                var opponentCarDataList = netPlayersData.AllPlayerDataList
                    .Where(d => d != netPlayersData.LocalPlayerData)
                    .Select(ToCarRaceModelData)
                    .ToArray();
                var opponentsCount = opponentCarDataList.Length;

                Debug.Log("Opponents count:" + opponentsCount);

                _raceModel = new NetRaceModel(
                    GetRaceDistance(opponentsCount),
                    playerCarData,
                    complexityData,
                    opponentCarDataList[0],
                    opponentsCount > 1 ? opponentCarDataList[1] : null,
                    opponentsCount > 2 ? opponentCarDataList[2] : null);
            }

            _modelsHolder.SetRaceModel(_raceModel);
        }

        private static int GetRaceDistance(int opponentsCount)
        {
            return opponentsCount * 500;
        }

        private CarRaceData CreateOpponentRaceData(int opponentIndex)
        {
            var opponentCarKey = _unlockedCars[_random.Next(_unlockedCars.Count)].CarKey;
            var carSettings = _carDataProvider.GetCarData(opponentCarKey);
            
            return new CarRaceData(
                carSettings,
                carPositionIndex: opponentIndex + 1,
                id: opponentIndex + 2);
        }

        private CarRaceData ToCarRaceModelData(P2PPlayerData p2pPlayerData)
        {
            var carData = _carDataProvider.GetCarData(p2pPlayerData.CarKey);
            return new CarRaceData(carData, p2pPlayerData.PositionIndex, p2pPlayerData.Id);
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
                InitChildController(new NetRacePlayerCarController(_raceModel.PlayerCar,
                    _contextView.CarContainerTransforms[_raceModel.PlayerCar.PositionIndex]));

                foreach (var opponentCarModel in _raceModel.OpponentCarModels)
                {
                    InitChildController(new NetRaceOpponentCarController(opponentCarModel,
                        _contextView.CarContainerTransforms[opponentCarModel.PositionIndex]));
                }

                InitChildController(new NetOpponentsProcessFinishController(_raceModel as NetRaceModel));
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