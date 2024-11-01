using System;
using Controller.MenuScene;
using Controller.RaceScene;
using Cysharp.Threading.Tasks;
using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine.SceneManagement;
using View.UI;

namespace Controller
{
    public class RootController : ControllerBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private readonly UILoadingOverlayView _loadingOverlayView;
        
        private ControllerBase _currentSceneRootController;

        public RootController(UILoadingOverlayView loadingOverlayView)
        {
            _loadingOverlayView = loadingOverlayView;
        }

        public override void Initialize()
        {
            LoadMenuScene();

            Subscribe();
        }

        public override void Dispose()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<RequestLoadSceneEvent>(OnRequestLoadSceneEvent);
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<RequestLoadSceneEvent>(OnRequestLoadSceneEvent);
        }

        private void OnRequestLoadSceneEvent(RequestLoadSceneEvent e)
        {
            LoadScene(e.SceneName).Forget();
        }

        private void LoadMenuScene()
        {
            LoadScene(Constants.MenuSceneName).Forget();
        }

        private async UniTaskVoid LoadScene(string sceneName)
        {
            await _loadingOverlayView.ShowLoadingOverlay();
            DisposeCurrentSceneRootController();

            await SceneManager.LoadSceneAsync(sceneName);
            _currentSceneRootController = RunSceneRootController(sceneName);
            
            await _loadingOverlayView.HideLoadingOverlay();
        }

        private void DisposeCurrentSceneRootController()
        {
            _currentSceneRootController?.Dispose();
            _currentSceneRootController = null;
        }

        private static ControllerBase RunSceneRootController(string sceneName)
        {
            ControllerBase result = sceneName switch
            {
                Constants.MenuSceneName => new MenuSceneRootController(),
                Constants.RaceSceneName => new RaceSceneRootController(),
                _ => throw new NotSupportedException($"Root controller for {sceneName} is not supported")
            };

            result.Initialize();

            return result;
        }
    }
}