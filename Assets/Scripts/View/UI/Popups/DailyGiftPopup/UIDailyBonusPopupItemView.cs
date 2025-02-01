using TMPro;
using UnityEngine;
using View.Extensions;
using View.UI.Common;

namespace View.UI.Popups.DailyBonusPopup
{
    public class UIDailyBonusPopupItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dayText;
        [SerializeField] private TMP_Text _cashAmountText;
        [SerializeField] private TMP_Text _goldAmountText;
        [SerializeField] private UITextButtonView _doubleRewardButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        private string _cashAmountTextRaw = string.Empty;
        private bool _goldTextVisibility = true;

        public UITextButtonView DoubleRewardButton => _doubleRewardButton;

        public void SetDayText(string dayText)
        {
            _dayText.text = dayText;
        }

        public void SetAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }
        
        public void SetCashAmountText(string cashAmountStr)
        {
            _cashAmountTextRaw = cashAmountStr;
            _cashAmountText.text = (_goldTextVisibility == false ? "\n" : "") + _cashAmountTextRaw;
        }
        
        public void SetGoldAmountText(string goldAmountStr)
        {
            _goldAmountText.text = goldAmountStr;
        }

        public void SetGoldTextVisibility(bool isVisible)
        {
            _goldTextVisibility = isVisible;
            _goldAmountText.gameObject.SetActive(_goldTextVisibility);

            UpdateCashAmountText();
        }

        public void SetDoubleRewardButtonVisibility(bool isVisible)
        {
            _doubleRewardButton.gameObject.SetActive(isVisible);
        }

        private void UpdateCashAmountText()
        {
            SetCashAmountText(_cashAmountTextRaw);
        }
    }
}