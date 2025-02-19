using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.UI.Common;

namespace View.UI.Popups.MultiplayerPopups
{
    public class UIMultiplayerJoinPopup : UIPopupViewBase
    {
        public event Action<string> JoinCodeValueChanged;
        
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private InputField _roomCodeText;
        [SerializeField] private Text _roomPlaceholderText;
        [SerializeField] private UITextButtonView _joinButton;
        [SerializeField] private UITextButtonView _pasteButton;

        public UITextButtonView JoinButton => _joinButton;
        public UITextButtonView PasteButton => _pasteButton;
        
        public string JoinCodeText => _roomCodeText.text;

        protected override void Awake()
        {
            base.Awake();
            
            _roomCodeText.onValueChanged.AddListener(OnCodeValueChanged);
        }

        public void SetMessageText(string text)
        {
            _messageText.text = text;
        }

        public void SetRoomCodePlaceholderText(string text)
        {
            _roomPlaceholderText.text = text;
        }

        public void SetRoomCodeVisibility(bool isVisible)
        {
            _roomCodeText.gameObject.SetActive(isVisible);
        }
        
        public void SetRoomCodeInteractable(bool isInteractable)
        {
            _roomCodeText.interactable = isInteractable;
        }

        public void SetRoomCodeText(string text)
        {
            _roomCodeText.text = text;
        }

        public void ResetRoomCodeText()
        {
            SetRoomCodeText(string.Empty);
        }

        private void OnCodeValueChanged(string value)
        {
            JoinCodeValueChanged?.Invoke(value);
        }
    }
}