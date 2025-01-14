using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Extensions;
using Infra.Instance;
using Providers.LocalizationProvider;
using UnityEngine;
using Utils.AudioManager;
using Utils.P2PRoomLib;
using View.UI.Popups.MultiplayerPopups;

namespace Controller.MenuScene.MultiplayerPopupControllers
{
    public class MenuSceneMultiplayerHostPopupController : ControllerBase
    {
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();

        private readonly RectTransform _targetTransform;
        
        private UIMultiplayerHostPopup _popupView;
        private P2PRoom _p2pRoom;

        public MenuSceneMultiplayerHostPopupController(RectTransform targetTransform)
        {
            _targetTransform = targetTransform;
        }

        public override void Initialize()
        {
            _popupView = Instantiate<UIMultiplayerHostPopup>(PrefabKey.UIHostPopup, _targetTransform);

            _popupView.SetMessageText(_localizationProvider.GetLocale(LocalizationKeys.HostPopupPreparingMessage));
            _popupView.SetRoomCodeVisibility(false);

            _popupView.Appear2Async()
                .ContinueWith(OnShown);
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
            
            _p2pRoom?.CancelJoiningLoop();
            
            Destroy(_popupView);
            _popupView = null;
        }

        private void OnShown()
        {
            Subscribe();

            ProcessCreateRoom().Forget();
        }

        private async UniTaskVoid ProcessCreateRoom()
        {
            _p2pRoom = new P2PRoom(Urls.P2PRoomsServiceUrl);

            var createRoomResult = await _p2pRoom.Create(2, CancellationToken.None);

            if (createRoomResult)
            {
                _popupView.SetMessageText(_localizationProvider.GetLocale(LocalizationKeys.HostPopupRoomReadyMessage));
                
                _popupView.SetRoomCodeText(_p2pRoom.RoomId.ToString());
                _popupView.SetRoomCodeVisibility(true);
            }
            else
            {
                _popupView.SetRoomCodeVisibility(false);
                _popupView.SetMessageText(_localizationProvider.GetLocale(LocalizationKeys.HostPopupErrorMessage));
            }
        }

        private void Subscribe()
        {
            _popupView.CloseButtonClicked += OnCloseButtonClicked;
        }

        private void Unsubscribe()
        {
            _popupView.CloseButtonClicked -= OnCloseButtonClicked;
        }

        private void OnCloseButtonClicked()
        {
            ProcessCloseButton().Forget();
        }

        private async UniTask ProcessCloseButton()
        {
            _audioPlayer.PlayButtonSound();
            
            _p2pRoom?.Dispose();
            _p2pRoom = null;
            
            await _popupView.Disappear2Async();
            
            RequestDispose();
        }
    }
}