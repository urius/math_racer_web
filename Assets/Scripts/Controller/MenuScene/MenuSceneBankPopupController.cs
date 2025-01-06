using Cysharp.Threading.Tasks;
using Data;
using Infra.Instance;
using Model;
using Providers;
using Providers.LocalizationProvider;
using UnityEngine;
using View.UI.Popups.BankPopup;

namespace Controller.MenuScene
{
    public class MenuSceneBankPopupController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        private readonly RectTransform _targetTransform;
        
        private UIBankPopup _bankPopup;
        private SessionDataModel _sessionDataModel;

        public MenuSceneBankPopupController(RectTransform targetTransform)
        {
            _targetTransform = targetTransform;
        }
        
        public override void Initialize()
        {
            _sessionDataModel = _modelsHolder.GetSessionDataModel();
            
            _sessionDataModel.IsBankPopupOpened.Value = true;
            
            _bankPopup = Instantiate<UIBankPopup>(PrefabKey.UIBankPopup, _targetTransform);
            
            _bankPopup.SetTitleText(_localizationProvider.GetLocale(LocalizationKeys.BankPopupTitle));
            _bankPopup.Setup(3, 840, 670);

            SetupContent();

            _bankPopup.AppearAsync()
                .ContinueWith(Subscribe);
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
            
        }

        private void Subscribe()
        {
            _bankPopup.CloseButtonClicked += OnCloseClicked;
        }

        private void Unsubscribe()
        {
            _bankPopup.CloseButtonClicked -= OnCloseClicked;
        }

        private void OnCloseClicked()
        {
            Unsubscribe();
            
            _bankPopup.DisappearAsync().ContinueWith(RequestDispose);
        }
    }
}