using System;
using System.Collections.Generic;
using System.Linq;
using Controller.Commands;
using Cysharp.Threading.Tasks;
using Data;
using Infra.CommandExecutor;
using Infra.Instance;
using Model;
using Providers;
using Providers.LocalizationProvider;
using UnityEngine;
using Utils;
using Utils.GamePush;
using View.UI.Popups.BankPopup;

namespace Controller.MenuScene
{
    public class MenuSceneBankPopupController : ControllerBase
    {
        private const int ColumnsAmount = 3;
        
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly ICommandExecutor _commandExecutor = Instance.Get<ICommandExecutor>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();

        private readonly PlayerModel _playerModel;
        private readonly SessionDataModel _sessionDataModel;
        private readonly RectTransform _targetTransform;
        private readonly List<ProductItemViewModelBase> _goldItemViewModels = new();
        private readonly List<ProductItemViewModelBase> _cashItemViewModels = new();
        private readonly BankAdWatchesModel _bankAdWatchesModel;

        private UIBankPopup _bankPopup;

        public MenuSceneBankPopupController(RectTransform targetTransform)
        {
            _targetTransform = targetTransform;
            
            _sessionDataModel = _modelsHolder.GetSessionDataModel();
            _bankAdWatchesModel = _sessionDataModel.BankAdWatches;
            _playerModel = _modelsHolder.GetPlayerModel();
        }
        
        private IEnumerable<ProductItemViewModelBase> AllItemViewModels
        {
            get
            {
                foreach (var itemViewModel in _cashItemViewModels)
                {
                    yield return itemViewModel;
                }

                foreach (var itemViewModel in _goldItemViewModels)
                {
                    yield return itemViewModel;
                }
            }
        }

        public override void Initialize()
        {
            _sessionDataModel.IsBankPopupOpened.Value = true;
            
            _bankPopup = Instantiate<UIBankPopup>(PrefabKey.UIBankPopup, _targetTransform);
            
            _bankPopup.SetTitleText(_localizationProvider.GetLocale(LocalizationKeys.BankPopupTitle));
            _bankPopup.Setup(ColumnsAmount, 840, 670);
            
            SetupViewModels();
            SetupContent();

            _bankPopup.AppearAsync()
                .ContinueWith(Subscribe);
        }

        private void SetupViewModels()
        {
            var isRewardedAdsAvailable = GamePushWrapper.CanShowRewardedAds();
            var purchasableProducts = _sessionDataModel.BankProductsData.Value;

            for (var i = 0; i < ColumnsAmount; i++)
            {
                if (i == 0 && isRewardedAdsAvailable)
                {
                    _goldItemViewModels.Add(new AdvertProductItemViewModel(Constants.ProductTypeGold, _bankAdWatchesModel));
                    _cashItemViewModels.Add(new AdvertProductItemViewModel(Constants.ProductTypeCash, _bankAdWatchesModel));
                }
                else
                {
                    if (purchasableProducts.GoldProducts.Count > i)
                    {
                        _goldItemViewModels.Add(new PurchaseProductItemViewModel(purchasableProducts.GoldProducts[i]));
                    }

                    if (purchasableProducts.CashProducts.Count > i)
                    {
                        _cashItemViewModels.Add(new PurchaseProductItemViewModel(purchasableProducts.CashProducts[i]));
                    }
                }
            }
        }

        public override void DisposeInternal()
        {
            _sessionDataModel.IsBankPopupOpened.Value = false;

            Unsubscribe();

            Destroy(_bankPopup);
            _bankPopup = null;
        }

        private void SetupContent()
        {
            for (var i = 0; i < ColumnsAmount; i++)
            {
                SetupGoldItemView(i);
                SetupCashItemView(i);
            }
        }

        private void SetupGoldItemView(int itemIndex)
        {
            if (_goldItemViewModels.Count > itemIndex)
            {
                var itemViewModel = _goldItemViewModels[itemIndex];
                var texts = GetTexts(itemViewModel);
                _bankPopup.SetupGoldItem(itemIndex, texts.AmountText, texts.BuyButtonText, GetCounter(itemViewModel));
                _bankPopup.SetGoldItemInteractable(itemIndex, itemViewModel.IsInteractable);
            }
            else
            {
                _bankPopup.SetupGoldItem(itemIndex, "-", _localizationProvider.GetLocale(LocalizationKeys.Unavailable),0);
                _bankPopup.SetGoldItemInteractable(itemIndex, isInteractable: false);
            }
        }
        
        private void SetupCashItemView(int itemIndex)
        {
            if (_cashItemViewModels.Count > itemIndex)
            {
                var itemViewModel = _cashItemViewModels[itemIndex];
                var texts = GetTexts(itemViewModel);
                _bankPopup.SetupCashItem(itemIndex, texts.AmountText, texts.BuyButtonText, GetCounter(itemViewModel));
                _bankPopup.SetCashItemInteractable(itemIndex, itemViewModel.IsInteractable);
            }
            else
            {
                _bankPopup.SetupCashItem(itemIndex, "-", _localizationProvider.GetLocale(LocalizationKeys.Unavailable), 0);
                _bankPopup.SetCashItemInteractable(itemIndex, isInteractable: false);
            }
        }

        private (string AmountText, string BuyButtonText) GetTexts(ProductItemViewModelBase itemViewModel)
        {
            string amountText;
            string buyButtonText;

            var isAdvert = itemViewModel.PurchaseType == ProductItemPurchaseType.Advert;

            if (isAdvert)
            {
                amountText = GetAdvertAmountText(itemViewModel);
                buyButtonText = _bankAdWatchesModel.AdWatchesRest > 0
                    ? $"{Constants.TextSpriteAds} {_localizationProvider.GetLocale(LocalizationKeys.WatchAd)}"
                    : GetAdCooldownTimeRestText();
            }
            else
            {
                var purchaseItemViewModel = (PurchaseProductItemViewModel)itemViewModel;
                amountText = purchaseItemViewModel.NameLocalized;
                buyButtonText =
                    $"{purchaseItemViewModel.Price} {GetCurrencyText(purchaseItemViewModel.Currency)}";
            }

            return (amountText, buyButtonText);
        }

        private string GetCurrencyText(string currencyName)
        {
            return GamePushWrapper.IsVKPlatform
                ? _localizationProvider.GetLocale(LocalizationKeys.CurrencyFormatPlural + currencyName.ToLower())
                : currencyName.ToUpper();
        }

        private int GetCounter(ProductItemViewModelBase itemViewModel)
        {
            var isAdvert = itemViewModel.PurchaseType == ProductItemPurchaseType.Advert;

            return isAdvert ? ((AdvertProductItemViewModel)itemViewModel).AdWatchesRest : -1;
        }

        private string GetAdCooldownTimeRestText()
        {
            return FormattingHelper.ToTimeFormatMinSec(Math.Max(0, _bankAdWatchesModel.AdWatchCooldownSecondsRest));
        }

        private string GetAdvertAmountText(ProductItemViewModelBase itemViewModel)
        {
            var gameCurrencyText = itemViewModel.ProductType == Constants.ProductTypeGold
                ? _localizationProvider.GetLocale(LocalizationKeys.Crystal)
                : _localizationProvider.GetLocale(LocalizationKeys.Cash);
            
            return $"{itemViewModel.ProductAmount} {gameCurrencyText}";
        }

        private void Subscribe()
        {
            _bankPopup.CloseButtonClicked += OnCloseClicked;
            _bankPopup.GoldItemBuyClicked += OnGoldItemBuyClicked;
            _bankPopup.CashItemBuyClicked += OnCashItemBuyClicked;
            
            _updatesProvider.GameplaySecondPassed += OnGameplaySecondPassed;
            _bankAdWatchesModel.AvailableWatchesCountUpdated += OnAvailableWatchesCountUpdated;
        }

        private void Unsubscribe()
        {
            _bankPopup.CloseButtonClicked -= OnCloseClicked;
            _bankPopup.GoldItemBuyClicked -= OnGoldItemBuyClicked;
            _bankPopup.CashItemBuyClicked -= OnCashItemBuyClicked;
            
            _updatesProvider.GameplaySecondPassed -= OnGameplaySecondPassed;
            _bankAdWatchesModel.AvailableWatchesCountUpdated -= OnAvailableWatchesCountUpdated;
        }

        private void OnAvailableWatchesCountUpdated()
        {
            UpdateAdItemViews();
        }

        private void OnGameplaySecondPassed()
        {
            if (_bankAdWatchesModel.AdWatchesRest <= 0)
            {
                UpdateAdItemViews();
            }
        }

        private void UpdateAdItemViews()
        {
            for (var i = 0; i < _goldItemViewModels.Count; i++)
            {
                if (_goldItemViewModels[i].PurchaseType == ProductItemPurchaseType.Advert)
                {
                    SetupGoldItemView(i);
                }
            }
            
            for (var i = 0; i < _cashItemViewModels.Count; i++)
            {
                if (_cashItemViewModels[i].PurchaseType == ProductItemPurchaseType.Advert)
                {
                    SetupCashItemView(i);
                }
            }
        }

        private void OnGoldItemBuyClicked(int index)
        {
            var itemViewModel = _goldItemViewModels[index];
            ProcessItemClicked(itemViewModel).Forget();
        }

        private void OnCashItemBuyClicked(int index)
        {
            var itemViewModel = _cashItemViewModels[index];
            ProcessItemClicked(itemViewModel).Forget();
        }

        private async UniTaskVoid ProcessItemClicked(ProductItemViewModelBase itemViewModel)
        {
            if (itemViewModel.PurchaseType == ProductItemPurchaseType.Advert)
            {
                if (GamePushWrapper.IsRewardedAdsShowInProgress) return;
                
                var showAdsSuccess = await GamePushWrapper.ShowRewardedAds();
                if (showAdsSuccess)
                {
                    if (itemViewModel.ProductType == Constants.ProductTypeGold)
                    {
                        _playerModel.AddGold(itemViewModel.ProductAmount);
                    }
                    else
                    {
                        _playerModel.AddCash(itemViewModel.ProductAmount);
                    }

                    _bankAdWatchesModel.HandleAdWatched();
                }
            }
            else if (itemViewModel.PurchaseType == ProductItemPurchaseType.Purchase)
            {
                var purchaseProductItemViewModel = (PurchaseProductItemViewModel)itemViewModel;
                var productTag = purchaseProductItemViewModel.Tag;
                
                var purchaseResult = await GamePushWrapper.PurchaseProduct(productTag);
                if (purchaseResult)
                {
                    await _commandExecutor.ExecuteAsync<ConsumeProductCommand, bool, string>(productTag);
                }
            }
        }

        private void OnCloseClicked()
        {
            Unsubscribe();
            
            _bankPopup.DisappearAsync().ContinueWith(RequestDispose);
        }

        private enum ProductItemPurchaseType
        {
            None = 0,
            Advert,
            Purchase,
        }

        private abstract class ProductItemViewModelBase
        {
            public readonly int ProductAmount;
            public readonly string ProductType;

            protected ProductItemViewModelBase(int productAmount, string productType)
            {
                ProductAmount = productAmount;
                ProductType = productType;
            }

            public abstract ProductItemPurchaseType PurchaseType { get; }
            public abstract bool IsInteractable { get; }
        }
        
        private class AdvertProductItemViewModel : ProductItemViewModelBase
        {
            private readonly BankAdWatchesModel _bankAdWatchesModel;

            public AdvertProductItemViewModel(string productType, BankAdWatchesModel bankAdWatchesModel)
                : base(productType == Constants.ProductTypeCash ? 100 : 1, productType)
            {
                _bankAdWatchesModel = bankAdWatchesModel;
            }

            public override ProductItemPurchaseType PurchaseType => ProductItemPurchaseType.Advert;
            public override bool IsInteractable => AdWatchesRest > 0;
            public int AdWatchesRest => _bankAdWatchesModel.AdWatchesRest;
        }
        
        private class PurchaseProductItemViewModel : ProductItemViewModelBase
        {
            private readonly BankProductData _bankProductData;

            public PurchaseProductItemViewModel(BankProductData bankProductData) 
                : base(bankProductData.ProductAmount, bankProductData.ProductType)
            {
                _bankProductData = bankProductData;
            }

            public override ProductItemPurchaseType PurchaseType => ProductItemPurchaseType.Purchase;
            public override bool IsInteractable => true;

            public int Id => _bankProductData.Id;
            public string Tag => _bankProductData.Tag;
            public int Price => _bankProductData.Price;
            public string Currency => _bankProductData.Currency;
            public string NameLocalized => _bankProductData.Name;
        }
    }
}