using Events;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Providers;
using UnityEngine.UI;
using View.UI.MenuScene;

namespace Controller.MenuScene
{
    public class MenuSceneDailyGiftButtonController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private readonly UIMenuSceneDailyGiftButtonView _dailyGiftButton;
        
        private PlayerModel _playerModel;

        public MenuSceneDailyGiftButtonController(UIMenuSceneDailyGiftButtonView dailyGiftButton)
        {
            _dailyGiftButton = dailyGiftButton;
        }

        public override void Initialize()
        {
            _playerModel = _modelsHolder.GetPlayerModel();

            if (_playerModel.SequentialDaysPlaying % 4 == 0 
                || _playerModel.SequentialDaysPlaying % 5 == 0)
            {
                _dailyGiftButton.SetBlueState();
            }
            else
            {
                _dailyGiftButton.SetGreenState();
            }

            UpdateDailyGiftButtonVisibility();

            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playerModel.DailyGiftTaken += OnDailyGiftTaken;
            
            _dailyGiftButton.Clicked += OnDailyGiftButtonClicked;
        }

        private void Unsubscribe()
        {
            _dailyGiftButton.Clicked -= OnDailyGiftButtonClicked;
            
            _playerModel.DailyGiftTaken -= OnDailyGiftTaken;
        }
        
        private void OnDailyGiftTaken()
        {
            UpdateDailyGiftButtonVisibility();
        }
        
        private void OnDailyGiftButtonClicked()
        {
            if (_playerModel.IsDailyGiftTaken) return;
            
            _eventBus.Dispatch(new UIRequestDailyGiftPopupEvent());
        }

        private void UpdateDailyGiftButtonVisibility()
        {
            _dailyGiftButton.SetVisibility(!_playerModel.IsDailyGiftTaken);
        }
    }
}