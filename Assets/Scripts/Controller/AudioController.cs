using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Providers;
using Utils.AudioManager;

namespace Controller
{
    public class AudioController : ControllerBase
    {
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IAudioClipsProvider _audioClipsProvider = Instance.Get<IAudioClipsProvider>();

        private UniTask _currentFadeOutMusicTask = UniTask.CompletedTask;
        private PlayerModel _playerModel;

        public override void Initialize()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            
            _audioPlayer.SetSettings(_playerModel.AudioSettingsModel);
            
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<StartUnloadCurrentSceneEvent>(OnStartUnloadCurrentSceneEvent);
            _eventBus.Subscribe<SceneLoadedEvent>(OnSceneLoadedEvent);
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<StartUnloadCurrentSceneEvent>(OnStartUnloadCurrentSceneEvent);
            _eventBus.Unsubscribe<SceneLoadedEvent>(OnSceneLoadedEvent);
        }

        private void OnStartUnloadCurrentSceneEvent(StartUnloadCurrentSceneEvent e)
        {
            _currentFadeOutMusicTask = _audioPlayer.FadeOutAndStopMusicAsync(CancellationToken.None);
        }

        private void OnSceneLoadedEvent(SceneLoadedEvent e)
        {
            PlaySceneMusic(e.SceneName).Forget();
        }

        private async UniTaskVoid PlaySceneMusic(string sceneName)
        {
            await _currentFadeOutMusicTask;
            
            switch (sceneName)
            {
                case Constants.MenuSceneName:
                    PlayMusic(MusicKey.MainMenu);
                    break;
            }
        }

        private void PlayMusic(MusicKey musicKey)
        {
            var clip = _audioClipsProvider.GetMusicByKey(musicKey);
            
            _audioPlayer.FadeInAndPlayMusicAsync(CancellationToken.None, clip, 1f);
        }
    }
}