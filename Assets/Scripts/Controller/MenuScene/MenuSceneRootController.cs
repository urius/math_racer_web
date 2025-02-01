using Controller.Common;
using Controller.MenuScene.MultiplayerPopupControllers;
using Events;
using Extensions;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Providers;
using Services;
using UnityEngine;
using Utils.AudioManager;
using View.UI.MenuScene;

namespace Controller.MenuScene
{
    public class MenuSceneRootController : ControllerBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IP2PRoomService _roomService = Instance.Get<IP2PRoomService>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        private UIMenuSceneRootCanvasView _rootCanvasView;
        private UIMenuSceneRootView _rootView;
        private SessionDataModel _sessionDataModel;

        public override void Initialize()
        {
            _roomService.DestroyCurrentRoom(); // finish p2p connections from prev game if any 
            
            _sessionDataModel = _modelsHolder.GetSessionDataModel();
            
            _rootCanvasView = Object.FindObjectOfType<UIMenuSceneRootCanvasView>();
            _rootView = Object.FindObjectOfType<UIMenuSceneRootView>();
            
            InitChildControllers();
            
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();

            _rootCanvasView = null;
        }

        private void InitChildControllers()
        {
            InitChildController(new MenuSceneRootViewController(_rootView));
            //UI
            InitChildController(new MenuSceneMoneyViewController(_rootCanvasView.MoneyCanvasView));
            InitChildController(new MenuSceneLevelViewController(_rootCanvasView.LevelCanvasView));
            InitChildController(new MenuSceneDailyGiftButtonController(_rootCanvasView.DailyGiftButton));
        }

        private void Subscribe()
        {
            _rootCanvasView.PlayButtonClicked += OnPlayButtonClicked;
            _rootCanvasView.MultiplayerButtonClicked += OnMultiplayerButtonClicked;
            _rootCanvasView.CarsButtonClicked += OnCarsButtonClicked;
            _rootCanvasView.SettingsButtonClicked += OnSettingsButtonClicked;
            
            _eventBus.Subscribe<UIRequestBankPopupEvent>(OnRequestBankPopupEvent);
            _eventBus.Subscribe<UISettingsPopupOpenedEvent>(OnUISettingsPopupOpenedEvent);
            _eventBus.Subscribe<UISettingsPopupClosedEvent>(OnUISettingsPopupClosedEvent);
            _eventBus.Subscribe<UIRequestDailyGiftPopupEvent>(OnUiRequestDailyGiftPopupEvent);
        }

        private void Unsubscribe()
        {
            _rootCanvasView.PlayButtonClicked -= OnPlayButtonClicked;
            _rootCanvasView.MultiplayerButtonClicked -= OnMultiplayerButtonClicked;
            _rootCanvasView.CarsButtonClicked -= OnCarsButtonClicked;
            _rootCanvasView.SettingsButtonClicked -= OnSettingsButtonClicked;
            
            _eventBus.Unsubscribe<UIRequestBankPopupEvent>(OnRequestBankPopupEvent);
            _eventBus.Unsubscribe<UISettingsPopupOpenedEvent>(OnUISettingsPopupOpenedEvent);
            _eventBus.Unsubscribe<UISettingsPopupClosedEvent>(OnUISettingsPopupClosedEvent);
            _eventBus.Unsubscribe<UIRequestDailyGiftPopupEvent>(OnUiRequestDailyGiftPopupEvent);
        }

        private void OnUISettingsPopupOpenedEvent(UISettingsPopupOpenedEvent e)
        {
            _rootCanvasView.SetSettingsButtonInteractable(false);
        }

        private void OnUISettingsPopupClosedEvent(UISettingsPopupClosedEvent e)
        {
            _rootCanvasView.SetSettingsButtonInteractable(true);
        }

        private void OnPlayButtonClicked()
        {
            _eventBus.Dispatch(new RequestNextSceneEvent());
            
            _audioPlayer.PlayButtonSound();
        }

        private void OnMultiplayerButtonClicked()
        {
            var multiplayerPopupController = new MenuSceneMultiplayerRootPopupController(_rootCanvasView.PopupsCanvasTransform);
            InitChildController(multiplayerPopupController);
            
            _audioPlayer.PlayButtonSound();
        }

        private void OnCarsButtonClicked()
        {
            var carsPopupController = new MenuSceneCarsPopupController(_rootCanvasView.PopupsCanvasTransform);
            InitChildController(carsPopupController);
            
            _audioPlayer.PlayButtonSound();
        }

        private void OnSettingsButtonClicked()
        {
            InitChildController(new SettingsPopupController(_rootCanvasView.PopupsCanvasTransform, isShortVersion: true));
            
            _audioPlayer.PlayButtonSound();
        }

        private void OnRequestBankPopupEvent(UIRequestBankPopupEvent e)
        {
            if (_sessionDataModel.IsBankPopupOpened.Value) return;
            
            var carsPopupController = new MenuSceneBankPopupController(_rootCanvasView.PopupsCanvasTransform);
            InitChildController(carsPopupController);
        }

        

        private void OnUiRequestDailyGiftPopupEvent(UIRequestDailyGiftPopupEvent e)
        {
            var dailyGiftPopupController = new MenuSceneDailyGiftPopupController(_rootCanvasView.PopupsCanvasTransform);
            InitChildController(dailyGiftPopupController);
        }
    }
}