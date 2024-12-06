using Events;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine;
using View.UI.MenuScene;

namespace Controller.MenuScene
{
    public class MenuSceneRootController : ControllerBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private UIMenuSceneRootCanvasView _rootCanvasView;

        public override void Initialize()
        {
            _rootCanvasView = Object.FindObjectOfType<UIMenuSceneRootCanvasView>();

            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();

            _rootCanvasView = null;
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