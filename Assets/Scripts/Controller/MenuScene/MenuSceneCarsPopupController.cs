using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Holders;
using Holders.LocalizationProvider;
using Infra.Instance;
using Model;
using UnityEngine;
using View.UI.Popups.CarsPopup;
using View.UI.Popups.ContentPopup;

namespace Controller.MenuScene
{
    public class MenuSceneCarsPopupController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly ICarDataProvider _carDataProvider = Instance.Get<ICarDataProvider>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        private readonly RectTransform _targetTransform;
        
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
                SetupItemView(itemView, carData);
                _popupView.AddItem(itemView);
            }
        }

        private void SetupItemView(UICarsPopupItemView itemView, CarData carData)
        {
            itemView.SetCarIconSprite(carData.IconSprite);
            itemView.SetParameterTexts(
                _localizationProvider.GetLocale(LocalizationKeys.AccelerationParameter),
                _localizationProvider.GetLocale(LocalizationKeys.MaxSpeedParameter));
            itemView.SetFirstParameterPercent(carData.AccelerationPercent);
            itemView.SetSecondParameterPercent(carData.MaxSpeedPercent);
            //todo
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
            await _popupView.DisappearAsync();
            
            RequestDispose();
        }
    }
}