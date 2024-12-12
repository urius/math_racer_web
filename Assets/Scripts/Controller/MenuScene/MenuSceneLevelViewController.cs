using System;
using Helpers;
using Infra.Instance;
using JetBrains.Annotations;
using Model;
using Model.RaceScene;
using Providers;
using View.UI.MenuScene;

namespace Controller.MenuScene
{
    public class MenuSceneLevelViewController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        
        private readonly UIMenuSceneLevelCanvasView _levelCanvasView;
        
        private PlayerModel _playerModel;
        [CanBeNull] private RaceModel _raceModel;
        private int _currentLevelExpAmount;
        private int _nextLevelExpAmount;

        public MenuSceneLevelViewController(UIMenuSceneLevelCanvasView levelCanvasView)
        {
            _levelCanvasView = levelCanvasView;
        }

        public override void Initialize()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            _raceModel = _modelsHolder.GetRaceModel();

            _currentLevelExpAmount = LevelPointsHelper.GetExpPointsForLevel(_playerModel.Level);
            _nextLevelExpAmount = LevelPointsHelper.GetExpPointsForLevel(_playerModel.Level + 1);

            ShowLevel();
        }

        private void ShowLevel()
        {
            var playerExpAmount = _playerModel.ExpAmount;
            _levelCanvasView.SetLevel(_playerModel.Level);
            ShowExpAmount(playerExpAmount);

            if (_raceModel?.RaceRewards != null)
            {
                LeanTween.cancel(_levelCanvasView.gameObject);
                var startExpToAnimate = (float)Math.Max(playerExpAmount - _raceModel.RaceRewards.ExpReward,
                    _currentLevelExpAmount);
                
                ShowExpAmount(startExpToAnimate);
                LeanTween.value(_levelCanvasView.gameObject, ShowExpAmount, startExpToAnimate, playerExpAmount,
                        1f)
                    .setEaseOutQuad()
                    .setDelay(0.5f);
            }
        }

        private void ShowExpAmount(float expAmount)
        {
            ShowExpAmount((int)expAmount);
        }

        private void ShowExpAmount(int expAmount)
        {
            var expPercent = (float)(expAmount - _currentLevelExpAmount) /
                             (_nextLevelExpAmount - _currentLevelExpAmount);
            _levelCanvasView.SetExpProgressLineXScale(expPercent);
        }

        public override void DisposeInternal()
        {
        }
    }
}