using Controller.Common;
using Data;
using Events;
using Extensions;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Providers;
using Providers.LocalizationProvider;
using UnityEngine;
using Utils.AudioManager;
using View.UI.MenuScene;

namespace Controller.MenuScene
{
    public class MenuSceneRootController : ControllerBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        private UIMenuSceneRootCanvasView _rootCanvasView;
        private UIMenuSceneRootView _rootView;
        private SessionDataModel _sessionDataModel;

        public override void Initialize()
        {
            _sessionDataModel = _modelsHolder.GetSessionDataModel();
            
            _rootCanvasView = Object.FindObjectOfType<UIMenuSceneRootCanvasView>();
            _rootView = Object.FindObjectOfType<UIMenuSceneRootView>();

            SetButtonTexts();
            
            InitChildControllers();
            
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();

            _rootCanvasView = null;
        }

        private void SetButtonTexts()
        {
            _rootCanvasView.SetPlayButtonText(_localizationProvider.GetLocale(LocalizationKeys.PlayButton));
            _rootCanvasView.SetCarsButtonText(_localizationProvider.GetLocale(LocalizationKeys.CarsButton));
        }

        private void InitChildControllers()
        {
            InitChildController(new MenuSceneRootViewController(_rootView));
            //UI
            InitChildController(new MenuSceneMoneyViewController(_rootCanvasView.MoneyCanvasView));
            InitChildController(new MenuSceneLevelViewController(_rootCanvasView.LevelCanvasView));
        }

        private void Subscribe()
        {
            _rootCanvasView.PlayButtonClicked += OnPlayButtonClicked;
            _rootCanvasView.CarsButtonClicked += OnCarsButtonClicked;
            _rootCanvasView.SettingsButtonClicked += OnSettingsButtonClicked;
            
            _eventBus.Subscribe<UIRequestBankPopupEvent>(OnRequestBankPopupEvent);
        }

        private void Unsubscribe()
        {
            _rootCanvasView.PlayButtonClicked -= OnPlayButtonClicked;
            _rootCanvasView.CarsButtonClicked -= OnCarsButtonClicked;
            _rootCanvasView.SettingsButtonClicked -= OnSettingsButtonClicked;
            
            _eventBus.Unsubscribe<UIRequestBankPopupEvent>(OnRequestBankPopupEvent);
        }

        private void OnPlayButtonClicked()
        {
            _eventBus.Dispatch(new RequestNextSceneEvent());
            
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
    }
}