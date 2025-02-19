using System.Collections.Generic;
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
        private readonly Dictionary<UICarsPopupItemView, CarSettings> _carDataByItemView = new();
        
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
            _popupView.Setup(1, 800, 645, new Vector2Int(0, -25));

            SetupContent();
            SetupContentPosition();

            _popupView.AppearAsync()
                .ContinueWith(Subscribe);
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
            
            Destroy(_popupView);
            _popupView = null;
        }

        private void SetupContentPosition()
        {
            foreach (var kvp in _carDataByItemView)
            {
                if (kvp.Value.CarKey == _playerModel.CurrentCar)
                {
                    var itemPos = kvp.Key.RectTransform.anchoredPosition;
                    _popupView.SetContentYPosition(-itemPos.y);
                    break;
                }
            }
        }

        private void SetupContent()
        {
            var dataToDisplay = _carDataProvider.GetUnlockedCarsByLevel(_playerModel.Level + 1);

            foreach (var carData in dataToDisplay)
            {
                var itemView = Instantiate<UICarsPopupItemView>(PrefabKey.UICarsPopupItem, _popupView.ContentTransform);
                _carDataByItemView[itemView] = carData;
                
                SetupItemView(itemView, carData);
                SubscribeOnItem(itemView);
                
                _popupView.AddItem(itemView);
            }
        }

        private void SetupItemView(UICarsPopupItemView itemView, CarSettings carSettings)
        {
            var isLocked = _playerModel.Level < carSettings.UnlockLevel;
            
            itemView.SetLockedState(isLocked);
            itemView.SetCarIconSprite(carSettings.IconSprite);

            if (isLocked == false)
            {
                itemView.SetSelectedState(_playerModel.CurrentCar == carSettings.CarKey);
                itemView.SetParameterTexts(
                    _localizationProvider.GetLocale(LocalizationKeys.AccelerationParameter),
                    _localizationProvider.GetLocale(LocalizationKeys.MaxSpeedParameter));
                itemView.SetFirstParameterPercent(carSettings.AccelerationPercent);
                itemView.SetSecondParameterPercent(carSettings.MaxSpeedPercent);

                itemView.SetButtonPriceText(
                    IsCarBought(carSettings)
                        ? _localizationProvider.GetLocale(LocalizationKeys.Choose)
                        : carSettings.Price.ToPriceView());
            }
            else
            {
                itemView.SetLockedText(_localizationProvider.GetLocale(LocalizationKeys.CarItemLockedText));
            }
        }

        private bool IsCarBought(CarSettings carSettings)
        {
            return _playerModel.IsCarBought(carSettings.CarKey);
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
        }

        private async UniTask ProcessCloseButton()
        {
            _audioPlayer.PlayButtonSound();
            
            await _popupView.DisappearAsync();
            
            RequestDispose();
        }
    }
}