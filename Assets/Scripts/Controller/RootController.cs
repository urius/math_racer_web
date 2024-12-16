using System;
using Controller.Commands;
using Controller.MenuScene;
using Controller.NewLevelScene;
using Controller.RaceScene;
using Cysharp.Threading.Tasks;
using Data;
using Events;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;
using UnityEngine.SceneManagement;
using View.UI;

namespace Controller
{
    public class RootController : ControllerBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ICommandExecutor _commandExecutor = Instance.Get<ICommandExecutor>();
        
        private readonly UILoadingOverlayView _loadingOverlayView;
        
        private ControllerBase _currentSceneRootController;
        private string _currentSceneName;

        public RootController(UILoadingOverlayView loadingOverlayView)
        {
            _loadingOverlayView = loadingOverlayView;
        }

        public override async void Initialize()
        {
            await InitPlayerModel();

            InitChildControllers();
            
            LoadScene(Constants.MenuSceneName).Forget();

            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void InitChildControllers()
        {
            InitChildController(new AudioController());
        }

        private UniTask InitPlayerModel()
        {
            return _commandExecutor.ExecuteAsync<InitPlayerModelCommand>();
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<RequestNextSceneEvent>(OnRequestNextSceneEvent);
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<RequestNextSceneEvent>(OnRequestNextSceneEvent);
        }

        private void OnRequestNextSceneEvent(RequestNextSceneEvent e)
        {
            var nextSceneName = string.Empty;

            if (e.SceneName == null)
            {
                //default next scene
                switch (_currentSceneName)
                {
                    case Constants.MenuSceneName:
                        nextSceneName = Constants.RaceSceneName;
                        break;
                    case Constants.RaceSceneName:
                    case Constants.NewLevelSceneName:
                        nextSceneName = Constants.MenuSceneName;
                        break;
                }
            }
            else
            {
                nextSceneName = e.SceneName;
            }

            LoadScene(nextSceneName).Forget();
        }

        private async UniTaskVoid LoadScene(string sceneName)
        {
            _eventBus.Dispatch(new StartUnloadCurrentSceneEvent());
            
            await _loadingOverlayView.ShowLoadingOverlay();
            DisposeCurrentSceneRootController();
            
            _eventBus.Dispatch(new StartLoadSceneEvent(sceneName));

            await SceneManager.LoadSceneAsync(sceneName);
            _currentSceneRootController = RunSceneRootController(sceneName);
            _currentSceneName = sceneName;
            
            await _loadingOverlayView.HideLoadingOverlay();
            
            _eventBus.Dispatch(new SceneLoadedEvent(sceneName));
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
                Constants.NewLevelSceneName => new NewLevelSceneRootController(),
                _ => throw new NotSupportedException($"Root controller for {sceneName} is not supported")
            };

            result.Initialize();

            return result;
        }
    }
}