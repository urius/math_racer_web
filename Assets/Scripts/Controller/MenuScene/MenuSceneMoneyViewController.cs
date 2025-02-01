using Data;
using Events;
using Extensions;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Providers;
using Utils.AudioManager;
using View.UI.MenuScene;

namespace Controller.MenuScene
{
    public class MenuSceneMoneyViewController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private readonly UIMenuSceneMoneyCanvasView _moneyCanvasView;
        
        private PlayerModel _playerModel;
        private SessionDataModel _sessionDataModel;

        public MenuSceneMoneyViewController(UIMenuSceneMoneyCanvasView moneyCanvasView)
        {
            _moneyCanvasView = moneyCanvasView;
        }
        
        public override void Initialize()
        {
            _sessionDataModel = _modelsHolder.GetSessionDataModel();
            _playerModel = _modelsHolder.GetPlayerModel();

            SetupMoneyAmount();

            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playerModel.CashAmountChanged += OnCashAmountChanged;
            _playerModel.GoldAmountChanged += OnGoldAmountChanged;
            _playerModel.InsufficientGold += OnInsufficientGold;
            _playerModel.InsufficientCash += OnInsufficientCash;
            _sessionDataModel.IsBankPopupOpened.ValueChanged += OnBankPopupOpenedValueChanged;
            
            _eventBus.Subscribe<UIRequestMoneyFlyAnimationEvent>(OnRequestMoneyFlyAnimationEvent);

            _moneyCanvasView.OpenBankButtonClicked += OnOpenBankButtonClicked;
        }
        
        private void Unsubscribe()
        {
            _playerModel.CashAmountChanged -= OnCashAmountChanged;
            _playerModel.GoldAmountChanged -= OnGoldAmountChanged;
            _playerModel.InsufficientGold -= OnInsufficientGold;
            _playerModel.InsufficientCash -= OnInsufficientCash;
            _sessionDataModel.IsBankPopupOpened.ValueChanged -= OnBankPopupOpenedValueChanged;
            
            _eventBus.Unsubscribe<UIRequestMoneyFlyAnimationEvent>(OnRequestMoneyFlyAnimationEvent);
            
            _moneyCanvasView.OpenBankButtonClicked -= OnOpenBankButtonClicked;
        }

        private void OnRequestMoneyFlyAnimationEvent(UIRequestMoneyFlyAnimationEvent e)
        {
            if (e.CashRewardAmount > 0)
            {
                _moneyCanvasView.AnimateFlyingCash(e.CashRewardAmount, e.ItemWorldPosition);
            }

            if (e.GoldRewardAmount > 0)
            {
                _moneyCanvasView.AnimateFlyingGold(e.GoldRewardAmount, e.ItemWorldPosition);
            }
        }

        private void OnBankPopupOpenedValueChanged(bool value, bool _)
        {
            if (value)
            {
                _moneyCanvasView.HideAddGoldButtons();
            }
            else
            {
                _moneyCanvasView.ShowAddGoldButtons();
            }
        }

        private void OnOpenBankButtonClicked()
        {
            _eventBus.Dispatch(new UIRequestBankPopupEvent());
            
            _audioPlayer.PlayButtonSound();
        }

        private void OnInsufficientGold(int neededAmount)
        {
            _moneyCanvasView.AnimateGoldRedBlink();
            
            _audioPlayer.PlaySound(SoundKey.Negative);
        }

        private void OnInsufficientCash(int neededAmount)
        {
            _moneyCanvasView.AnimateCashRedBlink();
            
            _audioPlayer.PlaySound(SoundKey.Negative);
        }

        private void OnCashAmountChanged(int deltaCash)
        {
            _moneyCanvasView.AnimateCashAmount(_playerModel.CashAmount);

            if (deltaCash > 0)
            {
                _audioPlayer.PlaySound(SoundKey.CashBox);
            }
        }

        private void OnGoldAmountChanged(int deltaGold)
        {
            _moneyCanvasView.AnimateGoldAmount(_playerModel.GoldAmount);
            
            if (deltaGold > 0)
            {
                _audioPlayer.PlaySound(SoundKey.CashBox);
            }
        }

        private void SetupMoneyAmount()
        {
            _moneyCanvasView.SetCashAmount(_playerModel.CashAmount);
            _moneyCanvasView.SetGoldAmount(_playerModel.GoldAmount);
        }
    }
}