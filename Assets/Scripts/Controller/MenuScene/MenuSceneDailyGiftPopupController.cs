using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Providers;
using Providers.LocalizationProvider;
using UnityEngine;
using Utils;
using Utils.GamePush;
using View.UI.Popups.DailyBonusPopup;

namespace Controller.MenuScene
{
    public class MenuSceneDailyGiftPopupController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private readonly RectTransform _targetTransform;
        private readonly List<GiftItemViewModel> _giftItemViewModels = new();
        
        private UIDailyBonusPopupView _popupView;
        private PlayerModel _playerModel;
        private bool _canShowRewardedAds;

        public MenuSceneDailyGiftPopupController(RectTransform targetTransform)
        {
            _targetTransform = targetTransform;
        }

        public override void Initialize()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            _canShowRewardedAds = GamePushWrapper.CanShowRewardedAds();

            _popupView = Instantiate<UIDailyBonusPopupView>(PrefabKey.UIDailyGiftPopup, _targetTransform);

            CreateViewModels();
            DisplayGiftItems();
            
            _popupView.AppearAsync()
                .ContinueWith(Subscribe);
        }

        public override void DisposeInternal()
        {
            Unsubscribe();

            Destroy(_popupView);
            _popupView = null;
        }

        private void DisplayGiftItems()
        {
            var dayText = _localizationProvider.GetLocale(LocalizationKeys.Day);
            
            for (var i = 0; i < _giftItemViewModels.Count; i++)
            {
                var vm = _giftItemViewModels[i];
                var itemView = _popupView.DailyBonusItems[i];
                itemView.SetDayText($"{dayText} {vm.DayNumber}");
                itemView.SetAlpha(vm.IsCurrentDay ? 1 : 0.5f);
                itemView.SetCashAmountText(
                    $"{Constants.TextSpriteCash}\n+{FormattingHelper.ToCommaSeparatedNumber(vm.CashRewardAmount)}");
                itemView.SetGoldTextVisibility(vm.GoldRewardAmount > 0);
                itemView.SetGoldAmountText($"+{vm.GoldRewardAmount} {Constants.TextSpriteCrystal}");
                itemView.SetDoubleRewardButtonVisibility(vm.IsCurrentDay);
                itemView.DoubleRewardButton.SetInteractable(!vm.IsDoubled && _canShowRewardedAds);
            }
        }

        private void Subscribe()
        {
            foreach (var item in _popupView.DailyBonusItems)
            {
                item.DoubleRewardButton.ButtonClicked += OnDoubleRewardButtonClicked;
            }
            
            _popupView.TakeRewardsButton.ButtonClicked += OnTakeRewardsButtonClicked;
        }

        private void Unsubscribe()
        {
            foreach (var item in _popupView.DailyBonusItems)
            {
                item.DoubleRewardButton.ButtonClicked -= OnDoubleRewardButtonClicked;
            }
            
            _popupView.TakeRewardsButton.ButtonClicked -= OnTakeRewardsButtonClicked;
        }

        private void CreateViewModels()
        {
            const int daysFrameLength = 5;
            var sequentialPlayingDayIndex = _playerModel.SequentialDaysPlaying - 1;
            var frameIndex = sequentialPlayingDayIndex / daysFrameLength;
            var firstDayInFrameNumber = frameIndex * daysFrameLength + 1;

            for (var i = 0; i < daysFrameLength; i++)
            {
                var playingDayNumber = firstDayInFrameNumber + i;
                var cashRewardAmount = GetDefaultCashRewardAmount(i) * (frameIndex + 1);
                var goldRewardAmount =  GetDefaultGoldRewardAmount(i) * (frameIndex + 1);
                var isCurrentDay = playingDayNumber == _playerModel.SequentialDaysPlaying;
                _giftItemViewModels.Add(new GiftItemViewModel(playingDayNumber, isCurrentDay, cashRewardAmount, goldRewardAmount));
            }
        }

        private static int GetDefaultCashRewardAmount(int dayIndex)
        {
            return dayIndex switch
            {
                0 => 100,
                1 => 500,
                2 => 1000,
                3 => 2000,
                4 => 5000,
                _ => 5000
            };
        }

        private static int GetDefaultGoldRewardAmount(int dayIndex)
        {
            return dayIndex switch
            {
                0 => 0,
                1 => 0,
                2 => 0,
                3 => 1,
                4 => 2,
                _ => 2
            };
        }

        private void OnDoubleRewardButtonClicked()
        {
            var currentDayGiftViewModel = _giftItemViewModels.FirstOrDefault(vm => vm.IsCurrentDay);
            if (currentDayGiftViewModel is { IsDoubled: false } 
                && _canShowRewardedAds
                && GamePushWrapper.IsRewardedAdsShowInProgress == false)
            {
                ProcessShowAdsAndUpdateReward().Forget();
            }
        }

        private async UniTaskVoid ProcessShowAdsAndUpdateReward()
        {
            var showAdsResult = await GamePushWrapper.ShowRewardedAds();
            if (showAdsResult)
            {
                var currentDayGiftViewModel = _giftItemViewModels.First(vm => vm.IsCurrentDay);
                currentDayGiftViewModel.DoubleReward();
                DisplayGiftItems();
            }
        }

        private void OnTakeRewardsButtonClicked()
        {
            _popupView.TakeRewardsButton.SetInteractable(false);
            Unsubscribe();
            
            var itemViewIndex = _giftItemViewModels.FindIndex(vm => vm.IsCurrentDay);
            var itemWorldPosition = _popupView.DailyBonusItems[itemViewIndex].transform.position;
            var itemViewModel = _giftItemViewModels[itemViewIndex];
            
            _eventBus.Dispatch(new UIRequestMoneyFlyAnimationEvent(itemViewModel.CashRewardAmount, itemViewModel.GoldRewardAmount, itemWorldPosition));
            
            _playerModel.AddCash(itemViewModel.CashRewardAmount);
            _playerModel.AddGold(itemViewModel.GoldRewardAmount);
            _playerModel.SetDailyGiftTaken();

            UniTask.Delay(500)
                .ContinueWith(_popupView.DisappearAsync)
                .ContinueWith(RequestDispose);

        }
        
        private class GiftItemViewModel
        {
            public readonly int DayNumber;
            public readonly bool IsCurrentDay;

            private readonly int _cashRewardAmountDefault;
            private readonly int _goldRewardAmountDefault;

            public GiftItemViewModel(int dayNumber, bool isCurrentDay, int cashRewardAmountDefault,
                int goldRewardAmountDefault)
            {
                DayNumber = dayNumber;
                IsCurrentDay = isCurrentDay;
                _cashRewardAmountDefault = cashRewardAmountDefault;
                _goldRewardAmountDefault = goldRewardAmountDefault;
            }
            
            public int CashRewardAmount => _cashRewardAmountDefault * (IsDoubled ? 2 : 1);
            public int GoldRewardAmount => _goldRewardAmountDefault * (IsDoubled ? 2 : 1);
            public bool IsDoubled { get; private set; }

            public void DoubleReward()
            {
                IsDoubled = true;
            }
        }
    }
}