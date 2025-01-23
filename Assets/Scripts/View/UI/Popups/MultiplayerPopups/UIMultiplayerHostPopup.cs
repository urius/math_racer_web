using TMPro;
using UnityEngine;
using View.UI.Common;

namespace View.UI.Popups.MultiplayerPopups
{
    public class UIMultiplayerHostPopup : UIPopupViewBase
    {
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private TMP_InputField _roomCodeText;
        [SerializeField] private UITextButtonView _copyButton;
        [SerializeField] private UITextButtonView _startGameButton;

        public UITextButtonView StartGameButton => _startGameButton;
        public UITextButtonView CopyButton => _copyButton;
        public string RoomCodeStr => _roomCodeText.text;

        public void SetMessageText(string text)
        {
            _messageText.text = text;
        }

        public void SetRoomCodeVisibility(bool isVisible)
        {
            _roomCodeText.gameObject.SetActive(isVisible);
        }

        public void SetRoomCodeText(string text)
        {
            _roomCodeText.text = text;
        }
    }
}