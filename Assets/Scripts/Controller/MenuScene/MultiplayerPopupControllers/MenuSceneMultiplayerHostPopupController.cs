using Cysharp.Threading.Tasks;
using Data;
using Extensions;
using Infra.Instance;
using Providers.LocalizationProvider;
using Services;
using UnityEngine;
using Utils.AudioManager;
using Utils.P2PLib;
using View.UI.Popups.MultiplayerPopups;

namespace Controller.MenuScene.MultiplayerPopupControllers
{
    public class MenuSceneMultiplayerHostPopupController : ControllerBase
    {
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IP2PRoomService _p2pRoomService = Instance.Get<IP2PRoomService>();

        private readonly RectTransform _targetTransform;
        
        private UIMultiplayerHostPopup _popupView;

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
            var createRoomResult = await _p2pRoomService.HostNewRoom();

            if (createRoomResult)
            {
                _popupView.SetMessageText(_localizationProvider.GetLocale(LocalizationKeys.HostPopupRoomReadyMessage));
                
                _popupView.SetRoomCodeText(_p2pRoomService.RoomId);
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
            _p2pRoomService.ConnectedPlayerReady += OnConnectedPlayerReady;
            _p2pRoomService.PlayerDisconnected += OnPlayerDisconnected;

            _popupView.StartGameButton.ButtonClicked += OnStartGameButtonClicked;
            _popupView.CloseButtonClicked += OnCloseButtonClicked;
        }

        private void Unsubscribe()
        {
            _p2pRoomService.ConnectedPlayerReady -= OnConnectedPlayerReady;
            _p2pRoomService.PlayerDisconnected -= OnPlayerDisconnected;
            
            _popupView.StartGameButton.ButtonClicked -= OnStartGameButtonClicked;
            _popupView.CloseButtonClicked -= OnCloseButtonClicked;
        }

        private void OnStartGameButtonClicked()
        {
            _p2pRoomService.IsJoinAllowed = false;

            _p2pRoomService.SendStartRace();
        }

        private void OnConnectedPlayerReady(IP2PConnection connection)
        {
            ShowPlayersConnectedMessage();
            UpdateStartButtonState();
        }

        private void OnPlayerDisconnected(IP2PConnection connection)
        {
            ShowPlayersConnectedMessage();
            UpdateStartButtonState();
        }

        private void UpdateStartButtonState()
        {
            _popupView.StartGameButton.SetInteractable(_p2pRoomService.ActiveConnections.Count > 0);
        }

        private void ShowPlayersConnectedMessage()
        {
            var message = _localizationProvider.GetLocale(LocalizationKeys.HostPopupRoomConnectedPlayersMessage);
            _popupView.SetMessageText(message + _p2pRoomService.ActiveConnections.Count);
        }

        private void OnCloseButtonClicked()
        {
            _audioPlayer.PlayButtonSound();

            _p2pRoomService.DestroyCurrentRoom(); // we destroy room only if player closes connection window

            _popupView.Disappear2Async()
                .ContinueWith(RequestDispose);
        }
    }
}