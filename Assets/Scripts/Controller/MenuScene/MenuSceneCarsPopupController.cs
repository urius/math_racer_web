using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Extensions;
using Infra.Instance;
using Model;
using Providers;
using Providers.LocalizationProvider;
using UnityEngine;
using Utils.AudioManager;
using View.Extensions;
using View.UI.Popups.CarsPopup;
using View.UI.Popups.ContentPopup;

namespace Controller.MenuScene
{
    public class MenuSceneCarsPopupController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly ICarDataProvider _carDataProvider = Instance.Get<ICarDataProvider>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        
        private readonly RectTransform _targetTransform;
        private readonly Dictionary<UICarsPopupItemView, CarData> _carDataByItemView = new();
        
        private UIContentPopup _popupView;
        private PlayerModel _playerModel;

        public MenuSceneCarsPopupController(RectTransform targetTransform)
        {
            _targetTransform = targetTransform;
        }

        public override void Initialize()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            
            _popupView = Instantiate<UIContentPopup>(PrefabKey.UIContentPopup, _targetTransform);
            _popupView.SetTitleText(_localizationProvider.GetLocale(LocalizationKeys.CarsPopupTitle));
            _popupView.Setup(1, 800, 670);

            SetupContent();

            _popupView.AppearAsync()
                .ContinueWith(Subscribe);
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
            
            Destroy(_popupView);
            _popupView = null;
        }

        private void SetupContent()
        {
            var dataToDisplay = _carDataProvider.CarDataList
                .Where(c => c.UnlockLevel <= _playerModel.Level + 1)
                .OrderBy(c => c.UnlockLevel)
                .ToArray();

            foreach (var carData in dataToDisplay)
            {
                var itemView = Instantiate<UICarsPopupItemView>(PrefabKey.UICarsPopupItem, _popupView.ContentTransform);
                _carDataByItemView[itemView] = carData;
                
                SetupItemView(itemView, carData);
                SubscribeOnItem(itemView);
                
                _popupView.AddItem(itemView);
            }
        }

        private void SetupItemView(UICarsPopupItemView itemView, CarData carData)
        {
            var isLocked = _playerModel.Level < carData.UnlockLevel;
            
            itemView.SetLockedState(isLocked);
            itemView.SetCarIconSprite(carData.IconSprite);

            if (isLocked == false)
            {
                itemView.SetSelectedState(_playerModel.CurrentCar == carData.CarKey);
                itemView.SetParameterTexts(
                    _localizationProvider.GetLocale(LocalizationKeys.AccelerationParameter),
                    _localizationProvider.GetLocale(LocalizationKeys.MaxSpeedParameter));
                itemView.SetFirstParameterPercent(carData.AccelerationPercent);
                itemView.SetSecondParameterPercent(carData.MaxSpeedPercent);

                itemView.SetButtonPriceText(
                    IsCarBought(carData)
                        ? _localizationProvider.GetLocale(LocalizationKeys.Choose)
                        : carData.Price.ToPriceView());
            }
            else
            {
                itemView.SetLockedText(_localizationProvider.GetLocale(LocalizationKeys.CarItemLockedText));
            }
        }

        private bool IsCarBought(CarData carData)
        {
            return _playerModel.IsCarBought(carData.CarKey);
        }

        private void SubscribeOnItem(UICarsPopupItemView itemView)
        {
            itemView.ButtonClicked -= OnItemButtonClicked;
            itemView.ButtonClicked += OnItemButtonClicked;
        }

        private void OnItemButtonClicked(UICarsPopupItemView targetView)
        {
            ProcessItemButtonClick(targetView);
        }

        private void ProcessItemButtonClick(UICarsPopupItemView targetView)
        {
            var carData = _carDataByItemView[targetView];
            
            if (IsCarBought(carData) == false 
                && _playerModel.TrySpend(carData.Price))
            {
                _playerModel.AddBoughtCar(carData.CarKey);
                
                _audioPlayer.PlaySound(SoundKey.CashBox);
            }

            if (IsCarBought(carData))
            {
                _playerModel.SetCurrentCar(carData.CarKey);
                UpdateItemViews();
                
                _audioPlayer.PlaySound(SoundKey.SelectCar);
            }
        }

        private void UpdateItemViews()
        {
            foreach (var kvp in _carDataByItemView)
            {
                SetupItemView(kvp.Key, kvp.Value);
            }
        }

        private void Subscribe()
        {
            _popupView.CloseButtonClicked += OnCloseButtonClicked;
        }

        private void Unsubscribe()
        {
            _popupView.CloseButtonClicked -= OnCloseButtonClicked;
        }

        private void OnCloseButtonClicked()
        {
            ProcessCloseButton().Forget();
            
            _audioPlayer.PlayButtonSound();
        }

        private async UniTask ProcessCloseButton()
        {
            await _popupView.DisappearAsync();
            
            RequestDispose();
        }
    }
}