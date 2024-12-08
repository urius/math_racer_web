using Holders;
using Infra.Instance;
using Model;
using View.UI.MenuScene;

namespace Controller.MenuScene
{
    public class MenuSceneMoneyViewController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        
        private readonly UIMenuSceneMoneyCanvasView _moneyCanvasView;
        
        private PlayerModel _playerModel;

        public MenuSceneMoneyViewController(UIMenuSceneMoneyCanvasView moneyCanvasView)
        {
            _moneyCanvasView = moneyCanvasView;
        }
        
        public override void Initialize()
        {
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
        }
        
        private void Unsubscribe()
        {
            _playerModel.CashAmountChanged -= OnCashAmountChanged;
            _playerModel.GoldAmountChanged -= OnGoldAmountChanged;
            _playerModel.InsufficientGold -= OnInsufficientGold;
            _playerModel.InsufficientCash -= OnInsufficientCash;
        }

        private void OnInsufficientGold(int neededAmount)
        {
            _moneyCanvasView.AnimateGoldRedBlink();
        }

        private void OnInsufficientCash(int neededAmount)
        {
            _moneyCanvasView.AnimateCashRedBlink();
        }

        private void OnCashAmountChanged(int deltaCash)
        {
            _moneyCanvasView.AnimateCashAmount(_playerModel.CashAmount);
        }

        private void OnGoldAmountChanged(int deltaGold)
        {
            _moneyCanvasView.AnimateGoldAmount(_playerModel.GoldAmount);
        }

        private void SetupMoneyAmount()
        {
            _moneyCanvasView.SetCashAmount(_playerModel.CashAmount);
            _moneyCanvasView.SetGoldAmount(_playerModel.GoldAmount);
        }
    }
}