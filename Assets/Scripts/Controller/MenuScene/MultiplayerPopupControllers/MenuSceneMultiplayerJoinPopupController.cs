using Cysharp.Threading.Tasks;
using Data;
using Infra.Instance;
using Providers.LocalizationProvider;
using UnityEngine;
using Utils.P2PRoomLib;
using View.UI.Popups.MultiplayerPopups;

namespace Controller.MenuScene.MultiplayerPopupControllers
{
    public class MenuSceneMultiplayerJoinPopupController : ControllerBase
    {
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly RectTransform _targetTransform;
        
        private UIMultiplayerJoinPopup _popupView;
        private P2PRoom _p2pRoom;

        public MenuSceneMultiplayerJoinPopupController(RectTransform targetTransform)
        {
            _targetTransform = targetTransform;
        }

        public override void Initialize()
        {
            _popupView = Instantiate<UIMultiplayerJoinPopup>(PrefabKey.UIJoinPopup, _targetTransform);

            _popupView.SetMessageText(_localizationProvider.GetLocale(LocalizationKeys.JoinPopupPreparingMessage));
            _popupView.SetRoomCodePlaceholderText(_localizationProvider.GetLocale(LocalizationKeys.EnterJoinCode));
            _popupView.SetRoomCodeVisibility(true);
            _popupView.ResetRoomCodeText();
            _popupView.JoinButton.SetText(_localizationProvider.GetLocale(LocalizationKeys.JoinButton));
            _popupView.JoinButton.SetInteractable(false);

            _popupView.Appear2Async()
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
            _popupView.JoinCodeValueChanged += OnJoinCodeValueChanged;
            _popupView.JoinButton.ButtonClicked += OnJoinButtonClicked;
        }

        private void Unsubscribe()
        {
            _popupView.CloseButtonClicked -= OnCloseButtonClicked;
            _popupView.JoinCodeValueChanged -= OnJoinCodeValueChanged;
            _popupView.JoinButton.ButtonClicked -= OnJoinButtonClicked;
        }

        private void OnJoinButtonClicked()
        {
            JoinRoom().Forget();
        }

        private async UniTaskVoid JoinRoom()
        {
            if (IsJoinCodeValid())
            {
                _popupView.SetRoomCodeInteractable(false);

                _p2pRoom = new P2PRoom(Urls.P2PRoomsServiceUrl);
                UpdateJoinButtonState();

                var joinResult = await _p2pRoom.Join(int.Parse(_popupView.JoinCodeText));
                if (joinResult)
                {
                    _popupView.SetRoomCodeVisibility(false);
                    _popupView.SetMessageText(
                        _localizationProvider.GetLocale(LocalizationKeys.JoinPopupWaitingStartMessage));
                }
                else
                {
                    _popupView.SetRoomCodeInteractable(true);

                    var messageWithError = _localizationProvider.GetLocale(LocalizationKeys.JoinPopupErrorMessage) +
                                           "\n" + _localizationProvider.GetLocale(LocalizationKeys.JoinPopupPreparingMessage);
                    _popupView.SetMessageText(messageWithError);
                    
                    _p2pRoom.Dispose();
                    _p2pRoom = null;
                }
            }

            UpdateJoinButtonState();
        }

        private void OnCloseButtonClicked()
        {
            Unsubscribe();
            
            if (_p2pRoom != null)
            {
                _p2pRoom.Dispose();
                _p2pRoom = null;
            }

            _popupView.Disappear2Async()
                .ContinueWith(RequestDispose);
        }

        private void OnJoinCodeValueChanged(string value)
        {
            UpdateJoinButtonState();
        }

        private void UpdateJoinButtonState()
        {
            _popupView.JoinButton.SetInteractable(IsJoinCodeValid() && _p2pRoom == null);
        }

        private bool IsJoinCodeValid()
        {
            var joinCodeStr = _popupView.JoinCodeText;
            
            return string.IsNullOrEmpty(joinCodeStr) == false
                && int.TryParse(joinCodeStr, out _);
        }
    }
}