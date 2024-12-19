using UnityEngine;
using View.UI.Common;

namespace View.UI.Popups.SettingsPopup
{
    public class UISettingsPopupView : UIPopupViewBase
    {
        [SerializeField] private UISliderContainerView _musicSlider;
        [SerializeField] private UISliderContainerView _soundsSlider;
        [SerializeField] private UITextButtonView _exitToMenuButton;

        public UISliderContainerView MusicSlider => _musicSlider;
        public UISliderContainerView SoundsSlider => _soundsSlider;
        public UITextButtonView ExitToMenuButton => _exitToMenuButton;

        public void SetExitButtonVisible(bool isVisible)
        {
            _exitToMenuButton.SetVisibility(isVisible);
        }
    }
}