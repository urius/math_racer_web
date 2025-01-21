using Cysharp.Threading.Tasks;
using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using Providers.LocalizationProvider;
using Services;
using UnityEngine;
using View.UI.Popups.MultiplayerPopups;

namespace Controller.MenuScene.MultiplayerPopupControllers
{
    public class MenuSceneMultiplayerJoinPopupController : ControllerBase
    {
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IP2PRoomService _p2pRoomService = Instance.Get<IP2PRoomService>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private readonly RectTransform _targetTransform;
        
        private UIMultiplayerJoinPopup _popupView;

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
            SetJoinButtonInteractable(false);

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
            _p2pRoomService.StartGameReceived += OnStartGameReceived;
            
            _popupView.CloseButtonClicked += OnCloseButtonClicked;
            _popupView.JoinCodeValueChanged += OnJoinCodeValueChanged;
            _popupView.JoinButton.ButtonClicked += OnJoinButtonClicked;
        }

        private void Unsubscribe()
        {
            _p2pRoomService.StartGameReceived -= OnStartGameReceived;
            
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
                SetJoinButtonInteractable(false);
                
                _popupView.SetMessageText(
                    _localizationProvider.GetLocale(LocalizationKeys.JoinPopupJoiningMessage));

                var joinResult = await _p2pRoomService.JoinRoom(int.Parse(_popupView.JoinCodeText));
                if (joinResult)
                {
                    _popupView.SetRoomCodeVisibility(false);
                    _popupView.SetMessageText(
                        _localizationProvider.GetLocale(LocalizationKeys.JoinPopupWaitingStartMessage));
                }
                else
                {
                    var messageWithError = _localizationProvider.GetLocale(LocalizationKeys.JoinPopupErrorMessage) +
                                           "\n" + _localizationProvider.GetLocale(LocalizationKeys.JoinPopupPreparingMessage);
                    _popupView.SetMessageText(messageWithError);
                    _popupView.SetRoomCodeInteractable(true);
                    
                    _p2pRoomService.DestroyCurrentRoom();
                }
                
                UpdateJoinButtonState();
            }
        }

        private void OnStartGameReceived()
        {
            Unsubscribe();
            
            _eventBus.Dispatch(
                new RequestNextSceneEvent(requestSceneParams: new RequestRaceSceneParams(isMultiplayer: true)));
        }

        private void OnCloseButtonClicked()
        {
            Unsubscribe();
            
            _p2pRoomService.DestroyCurrentRoom();

            _popupView.Disappear2Async()
                .ContinueWith(RequestDispose);
        }

        private void OnJoinCodeValueChanged(string value)
        {
            UpdateJoinButtonState();
        }

        private void UpdateJoinButtonState()
        {
            SetJoinButtonInteractable(IsJoinCodeValid() && _p2pRoomService.HasRoom == false);
        }

        private void SetJoinButtonInteractable(bool isInteractable)
        {
            _popupView.JoinButton.SetInteractable(isInteractable);
        }

        private bool IsJoinCodeValid()
        {
            var joinCodeStr = _popupView.JoinCodeText;
            
            return string.IsNullOrEmpty(joinCodeStr) == false
                && int.TryParse(joinCodeStr, out _);
        }
    }
}