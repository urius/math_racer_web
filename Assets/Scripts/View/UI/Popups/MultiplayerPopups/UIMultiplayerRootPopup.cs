using UnityEngine;
using View.UI.Common;

namespace View.UI.Popups.MultiplayerPopups
{
    public class UIMultiplayerRootPopup : UIPopupViewBase
    {
        [SerializeField] private UITextButtonView _hostGameButton;
        [SerializeField] private UITextButtonView _joinGameButton;
        [SerializeField] private UITextButtonView _randomGameButton;

        public UITextButtonView HostGameButton => _hostGameButton;
        public UITextButtonView JoinGameButton => _joinGameButton;
        public UITextButtonView RandomGameButton => _randomGameButton;
    }
}