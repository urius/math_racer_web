using Data;
using Events;
using Extensions;
using Infra.EventBus;
using Infra.Instance;
using Providers.LocalizationProvider;
using UnityEngine;
using Utils.AudioManager;
using View.UI.MenuScene;

namespace Controller.MenuScene
{
    public class MenuSceneRootController : ControllerBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        private UIMenuSceneRootCanvasView _rootCanvasView;
        private UIMenuSceneRootView _rootView;

        public override void Initialize()
        {
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
        }

        private void Unsubscribe()
        {
            _rootCanvasView.PlayButtonClicked -= OnPlayButtonClicked;
            _rootCanvasView.CarsButtonClicked -= OnCarsButtonClicked;
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
    }
}