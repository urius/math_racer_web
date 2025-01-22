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
            
            _eventBus.Subscribe<RaceFinishingEvent>(OnRaceFinishingEvent);
            _eventBus.Subscribe<RequestFinishMusicEvent>(OnRequestFinishMusicEvent);
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<StartUnloadCurrentSceneEvent>(OnStartUnloadCurrentSceneEvent);
            _eventBus.Unsubscribe<SceneLoadedEvent>(OnSceneLoadedEvent);
            
            _eventBus.Unsubscribe<RaceFinishingEvent>(OnRaceFinishingEvent);
            _eventBus.Unsubscribe<RequestFinishMusicEvent>(OnRequestFinishMusicEvent);
        }

        private void OnStartUnloadCurrentSceneEvent(StartUnloadCurrentSceneEvent e)
        {
            if (e.NextSceneName == Constants.NewLevelSceneName) return;

            FadeOutCurrentMusic();
        }

        private void OnRaceFinishingEvent(RaceFinishingEvent e)
        {
            FadeOutCurrentMusic(duration: 0.1f);
        }

        private void FadeOutCurrentMusic(float duration = 0.5f)
        {
            _currentFadeOutMusicTask = _audioPlayer.FadeOutAndStopMusicAsync(CancellationToken.None, duration);
        }

        private void OnRequestFinishMusicEvent(RequestFinishMusicEvent e)
        {
            _currentFadeOutMusicTask.ContinueWith(() => PlayMusic(MusicKey.FinishRace, 0.1f));
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
                case Constants.RaceSceneName:
                    PlayMusic(MusicKey.Race_1);
                    break;
            }
        }

        private void PlayMusic(MusicKey musicKey, float duration = 1f)
        {
            var clip = _audioClipsProvider.GetMusicByKey(musicKey);
            
            _audioPlayer.FadeInAndPlayMusicAsync(CancellationToken.None, clip, duration);
        }
    }
}