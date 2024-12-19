using Cysharp.Threading.Tasks;
using Data;
using Events;
using Extensions;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Providers;
using UnityEngine;
using Utils.AudioManager;
using View.UI.Popups.SettingsPopup;

namespace Controller.Common
{
    public class SettingsPopupController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private readonly RectTransform _targetTransform;
        private readonly bool _isShortVersion;
        
        private UISettingsPopupView _popupView;
        private AudioSettingsModel _audioSettingsModel;

        public SettingsPopupController(RectTransform targetTransform, bool isShortVersion = false)
        {
            _targetTransform = targetTransform;
            _isShortVersion = isShortVersion;
        }
        
        public override void Initialize()
        {
            _audioSettingsModel = _modelsHolder.GetPlayerModel().AudioSettingsModel;
            
            _popupView = Instantiate<UISettingsPopupView>(PrefabKey.UISettingsPopup, _targetTransform);

            SetupView();

            Subscribe();

            _popupView.Appear2Async().Forget();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
            
            Destroy(_popupView);
            _popupView = null;
        }

        private void Subscribe()
        {
            _popupView.CloseButtonClicked += OnCloseButtonClicked;
            _popupView.MusicSlider.SliderValueChanged += OnMusicSliderValueChanged;
            _popupView.SoundsSlider.SliderValueChanged += OnSoundsSliderValueChanged;
            _popupView.ExitToMenuButton.ButtonClicked += OnExitToMenuClicked;
        }

        private void Unsubscribe()
        {
            _popupView.CloseButtonClicked -= OnCloseButtonClicked;
            _popupView.MusicSlider.SliderValueChanged -= OnMusicSliderValueChanged;
            _popupView.SoundsSlider.SliderValueChanged -= OnSoundsSliderValueChanged;
            _popupView.ExitToMenuButton.ButtonClicked -= OnExitToMenuClicked;
        }

        private void OnExitToMenuClicked()
        {
            _eventBus.Dispatch(new RequestNextSceneEvent(Constants.MenuSceneName));
            
            _audioPlayer.PlayButtonSound();
        }

        private void OnMusicSliderValueChanged(float value)
        {
            _audioSettingsModel.SetMusicVolume(value);
        }

        private void OnSoundsSliderValueChanged(float value)
        {
            _audioSettingsModel.SetSoundsVolume(value);
        }

        private void OnCloseButtonClicked()
        {
            Unsubscribe();
            
            _audioPlayer.PlayButtonSound();
            
            _popupView.Disappear2Async()
                .ContinueWith(RequestDispose);
        }

        private void SetupView()
        {
            _popupView.MusicSlider.SetSliderValue(_audioSettingsModel.MusicVolume);
            _popupView.SoundsSlider.SetSliderValue(_audioSettingsModel.SoundsVolume);

            _popupView.SetExitButtonVisible(!_isShortVersion);
            _popupView.SetPopupSize(500, _isShortVersion ? 300 : 400);
        }
    }
}