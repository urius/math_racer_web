using System;
using UnityEngine;
using View.UI.Popups.ContentPopup;

namespace View.UI.Popups.BankPopup
{
    public class UIBankPopup : UIContentPopup
    {
        public event Action<int> GoldItemBuyClicked;
        public event Action<int> CashItemBuyClicked;
        
        [SerializeField] private UIBankPopupItemView[] _goldItems;
        [SerializeField] private UIBankPopupItemView[] _cashItems;

        protected override void Awake()
        {
            base.Awake();

            Subscribe();
        }

        protected override void OnDestroy()
        {
            Unsubscribe();
            
            base.OnDestroy();
        }

        public void SetupGoldItem(int index, string amountText, string buyButtonText)
        {
            SetupItem(_goldItems[index], amountText, buyButtonText);
        }

        public void SetGoldItemInteractable(int index, bool isInteractable)
        {
            _goldItems[index].BuyButton.SetInteractable(isInteractable);
        }
        
        public void SetupCashItem(int index, string amountText, string buyButtonText)
        {
            SetupItem(_cashItems[index], amountText, buyButtonText);
        }

        public void SetCashItemInteractable(int index, bool isInteractable)
        {
            _cashItems[index].BuyButton.SetInteractable(isInteractable);
        }

        private void SetupItem(UIBankPopupItemView itemView, string amountText, string buyButtonText)
        {
            itemView.SetAmountText(amountText);
            itemView.BuyButton.SetText(buyButtonText);
        }

        private void Subscribe()
        {
            foreach (var goldItem in _goldItems)
            {
                goldItem.BuyButtonClicked += OnBuyGoldButtonClicked;
            }

            foreach (var cashItem in _cashItems)
            {
                cashItem.BuyButtonClicked += OnBuyCashButtonClicked;
            }
        }

        private void Unsubscribe()
        {
            foreach (var goldItem in _goldItems)
            {
                goldItem.BuyButtonClicked -= OnBuyGoldButtonClicked;
            }

            foreach (var cashItem in _cashItems)
            {
                cashItem.BuyButtonClicked -= OnBuyCashButtonClicked;
            }
        }

        private void OnBuyGoldButtonClicked(UIBankPopupItemView itemView)
        {
            var index = Array.IndexOf(_goldItems, itemView);

            Debug.Log($"{nameof(OnBuyGoldButtonClicked)} {index}");
            
            GoldItemBuyClicked?.Invoke(index);
        }

        private void OnBuyCashButtonClicked(UIBankPopupItemView itemView)
        {
            var index = Array.IndexOf(_cashItems, itemView);
            
            Debug.Log($"{nameof(OnBuyCashButtonClicked)} {index}");
            
            CashItemBuyClicked?.Invoke(index);
        }
    }
}