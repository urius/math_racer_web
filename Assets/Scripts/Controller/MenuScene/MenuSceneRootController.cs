using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine;
using View.UI;
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

        public override void Dispose()
        {
            Unsubscribe();

            _rootCanvasView = null;
        }

        private void Subscribe()
        {
            _rootCanvasView.PlayButtonClicked += OnPlayButtonClicked;
        }

        private void Unsubscribe()
        {
            _rootCanvasView.PlayButtonClicked -= OnPlayButtonClicked;
        }

        private void OnPlayButtonClicked()
        {
            _eventBus.Dispatch(new RequestLoadSceneEvent(Constants.RaceSceneName));
        }
    }
}