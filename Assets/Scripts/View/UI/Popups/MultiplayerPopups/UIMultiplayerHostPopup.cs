using TMPro;
using UnityEngine;

namespace View.UI.Popups.MultiplayerPopups
{
    public class UIMultiplayerHostPopup : UIPopupViewBase
    {
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private TMP_InputField _roomCodeText;

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