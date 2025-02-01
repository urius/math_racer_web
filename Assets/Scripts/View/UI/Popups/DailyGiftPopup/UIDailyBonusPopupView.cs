using UnityEngine;
using View.UI.Common;

namespace View.UI.Popups.DailyBonusPopup
{
    public class UIDailyBonusPopupView : UIPopupViewBase
    {
        [SerializeField] private UIDailyBonusPopupItemView[] _dailyBonusItems;
        [SerializeField] private UITextButtonView _takeRewardsButton;
        
        public UIDailyBonusPopupItemView[] DailyBonusItems => _dailyBonusItems;
        public UITextButtonView TakeRewardsButton => _takeRewardsButton;
    }
}