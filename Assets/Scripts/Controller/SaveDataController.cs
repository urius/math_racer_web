using Controller.Commands;
using Data;
using Events;
using Infra.CommandExecutor;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Providers;

namespace Controller
{
    public class SaveDataController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly ICommandExecutor _commandExecutor = Instance.Get<ICommandExecutor>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        
        private readonly PlayerModel _playerModel;
        
        private bool _needSaveFlag = false;

        public SaveDataController()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
        }
        
        public override void Initialize()
        {
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _eventBus.Subscribe<RequestSaveDataEvent>(OnRequestSaveDataEvent);
            _updatesProvider.RealtimeSecondPassed += OnRealtimeSecondPassed;

            _playerModel.CashAmountChanged += OnCashAmountChanged;
            _playerModel.GoldAmountChanged += OnGoldAmountChanged;
            _playerModel.ExpAmountChanged += OnExpAmountChanged;
            _playerModel.CurrentCarUpdated += OnCurrentCarUpdated;
            _playerModel.AudioSettingsModel.MusicVolumeChanged += OnAudioSettingsChanged;
            _playerModel.AudioSettingsModel.SoundsVolumeChanged += OnAudioSettingsChanged;
            _playerModel.AudioSettingsModel.MusicMutedStateChanged += OnAudioMuteSettingsChanged;
            _playerModel.AudioSettingsModel.SoundsMutedStateChanged += OnAudioMuteSettingsChanged;
        }

        private void Unsubscribe()
        {
            _eventBus.Unsubscribe<RequestSaveDataEvent>(OnRequestSaveDataEvent);
            _updatesProvider.RealtimeSecondPassed -= OnRealtimeSecondPassed;

            _playerModel.CashAmountChanged -= OnCashAmountChanged;
            _playerModel.GoldAmountChanged -= OnGoldAmountChanged;
            _playerModel.ExpAmountChanged -= OnExpAmountChanged;
            _playerModel.CurrentCarUpdated -= OnCurrentCarUpdated;
            _playerModel.AudioSettingsModel.MusicVolumeChanged -= OnAudioSettingsChanged;
            _playerModel.AudioSettingsModel.SoundsVolumeChanged -= OnAudioSettingsChanged;
            _playerModel.AudioSettingsModel.MusicMutedStateChanged -= OnAudioMuteSettingsChanged;
            _playerModel.AudioSettingsModel.SoundsMutedStateChanged -= OnAudioMuteSettingsChanged;
        }

        private void OnAudioMuteSettingsChanged(bool _)
        {
            MarkSaveFlag();
        }

        private void OnAudioSettingsChanged(float _)
        {
            MarkSaveFlag();
        }

        private void OnCurrentCarUpdated(CarKey _)
        {
            MarkSaveFlag();
        }

        private void OnExpAmountChanged(int _)
        {
            MarkSaveFlag();
        }

        private void OnGoldAmountChanged(int _)
        {
            MarkSaveFlag();
        }

        private void OnCashAmountChanged(int _)
        {
            MarkSaveFlag();
        }

        private void MarkSaveFlag()
        {
            _needSaveFlag = true;
        }

        private void OnRequestSaveDataEvent(RequestSaveDataEvent e)
        {
            ProcessSave();
        }

        private void OnRealtimeSecondPassed()
        {
            if (_needSaveFlag)
            {
                ProcessSave();
            }
        }

        private void ProcessSave()
        {
            _commandExecutor.Execute<SavePlayerDataCommand>();
            _needSaveFlag = false;
        }
    }
}