using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using Providers.LocalizationProvider;
using UnityEngine;
using View.UI.MenuScene;

namespace Controller.MenuScene
{
    public class MenuSceneRootController : ControllerBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
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
            InitChildController(new MenuSceneMoneyViewController(_rootCanvasView.MoneyCanvasView));
            InitChildController(new MenuSceneRootViewController(_rootView));
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
        }

        private void OnCarsButtonClicked()
        {
            var carsPopupController = new MenuSceneCarsPopupController(_rootCanvasView.PopupsCanvasTransform);
            InitChildController(carsPopupController);
        }
    }
}