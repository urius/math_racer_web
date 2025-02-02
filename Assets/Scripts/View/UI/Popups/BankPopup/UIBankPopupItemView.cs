using System;
using TMPro;
using UnityEngine;
using View.UI.Common;

namespace View.UI.Popups.BankPopup
{
    public class UIBankPopupItemView : MonoBehaviour
    {
        public event Action<UIBankPopupItemView> BuyButtonClicked;
        
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private UITextButtonView _buyButton;
        [SerializeField] private UICounterView _counterView;

        public UITextButtonView BuyButton => _buyButton;

        private void Awake()
        {
            _buyButton.ButtonClicked += OnBuyButtonClicked;
        }

        private void OnDestroy()
        {
            _buyButton.ButtonClicked -= OnBuyButtonClicked;
        }

        public void SetCounter(int amount)
        {
            _counterView.SetVisibility(amount > 0);
            _counterView.SetCounterText(amount.ToString());
        }

        public void SetAmountText(string text)
        {
            _amountText.text = text;
        }

        private void OnBuyButtonClicked()
        {
            BuyButtonClicked?.Invoke(this);
        }
    }
}