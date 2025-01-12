using Cysharp.Threading.Tasks;
using Data;
using Extensions;
using Infra.Instance;
using UnityEngine;
using Utils.AudioManager;
using View.UI.Popups.MultiplayerPopups;

namespace Controller.MenuScene
{
    public class MenuSceneMultiplayerRootPopupController : ControllerBase
    {
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();

        private readonly RectTransform _targetTransform;
        
        private UIMultiplayerRootPopup _popupView;

        public MenuSceneMultiplayerRootPopupController(RectTransform targetTransform)
        {
            _targetTransform = targetTransform;
        }

        public override void Initialize()
        {
            _popupView = Instantiate<UIMultiplayerRootPopup>(PrefabKey.UIMultiplayerRootPopup, _targetTransform);

            _popupView.AppearAsync()
                .ContinueWith(Subscribe);
        }

        public override void DisposeInternal()
        {
            Destroy(_popupView);
            _popupView = null;
        }

        private void Subscribe()
        {
            _popupView.CloseButtonClicked += OnCloseButtonClicked;
            
            _popupView.HostGameButton.ButtonClicked += OnHostButtonClicked;
            _popupView.JoinGameButton.ButtonClicked += OnJoinButtonClicked;
            _popupView.RandomGameButton.ButtonClicked += OnRandomButtonClicked;
        }
        
        private void Unsubscribe()
        {
            _popupView.CloseButtonClicked -= OnCloseButtonClicked;
            
            _popupView.HostGameButton.ButtonClicked -= OnHostButtonClicked;
            _popupView.JoinGameButton.ButtonClicked -= OnJoinButtonClicked;
            _popupView.RandomGameButton.ButtonClicked -= OnRandomButtonClicked;
        }

        private void OnCloseButtonClicked()
        {
            ProcessCloseButton().Forget();
        }

        private async UniTask ProcessCloseButton()
        {
            _audioPlayer.PlayButtonSound();
            
            await _popupView.DisappearAsync();
            
            RequestDispose();
        }

        private void OnHostButtonClicked()
        {
            
        }

        private void OnJoinButtonClicked()
        {
            
        }

        private void OnRandomButtonClicked()
        {
            
        }
    }
}