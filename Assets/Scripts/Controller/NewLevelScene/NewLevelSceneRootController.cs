using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Events;
using Extensions;
using Helpers;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.RaceScene;
using Providers;
using Providers.LocalizationProvider;
using UnityEngine;
using Utils;
using Utils.AudioManager;
using View.Extensions;
using View.UI.NewLevelScene;
using Object = UnityEngine.Object;

namespace Controller.NewLevelScene
{
    public class NewLevelSceneRootController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly ICarDataProvider _carDataProvider = Instance.Get<ICarDataProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();

        private readonly AnimateExpContext _animateExpContext = new AnimateExpContext();
        
        private Action _expProgressLineIsFullAction;
        private UINewLevelSceneRootCanvasView _rootView;
        private PlayerModel _playerModel;
        private RaceModel _raceModel;
        
        private int _prevLevel;
        private int _prevExpAmount;
        private int _currentLevelExpTarget;
        private int _prevLevelExpTarget;
        private int _nextLevelExpTarget;
        private bool _isLevelUp;

        public override void Initialize()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            _raceModel = _modelsHolder.GetRaceModel();
            InitTempData();

            _rootView = Object.FindObjectOfType<UINewLevelSceneRootCanvasView>();

            SetupView();

            RunNewLevelFlow().Forget();

            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }
        

        private void Subscribe()
        {
            _rootView.ContinueButton.ButtonClicked += OnContinueClicked;
        }

        private void Unsubscribe()
        {
            _rootView.ContinueButton.ButtonClicked -= OnContinueClicked;
        }

        private void OnContinueClicked()
        {
            _eventBus.Dispatch(new RequestNextSceneEvent());
            
            _audioPlayer.PlayButtonSound();
        }

        private void InitTempData()
        {
            _prevExpAmount = _playerModel.ExpAmount - _raceModel.RaceRewards.ExpReward;
            _prevLevel = LevelPointsHelper.GetLevelByExpPointsAmount(_prevExpAmount);
            _currentLevelExpTarget = LevelPointsHelper.GetExpPointsForLevel(_playerModel.Level);
            _prevLevelExpTarget = LevelPointsHelper.GetExpPointsForLevel(_prevLevel);
            _nextLevelExpTarget = LevelPointsHelper.GetExpPointsForLevel(_playerModel.Level + 1);
            _isLevelUp = _prevLevel < _playerModel.Level;
        }

        private void SetupView()
        {
            var levelText = _localizationProvider.GetLocale(LocalizationKeys.Level);
            if (_isLevelUp)
            {
                _rootView.SetLevelText1($"{levelText} {_prevLevel}");
                _rootView.SetLevelText2($"{levelText} {_playerModel.Level}");
            }
            else
            {
                _rootView.SetLevelText1($"{levelText} {_playerModel.Level}");
                _rootView.SetLevelText2($"{levelText} {_playerModel.Level + 1}");
            }

            SetExpAmount(_prevExpAmount);
            
            _rootView.SetCongratsText(_localizationProvider.GetLocale(LocalizationKeys.NewLevelCongrats));
            _rootView.SetNewCarsText(_localizationProvider.GetLocale(LocalizationKeys.NewCarsAvailable));
            _rootView.ContinueButton.SetText(_localizationProvider.GetLocale(LocalizationKeys.ContinueButton));
        }

        private void SetExpAmount(float expFloat)
        {
            var expAmountInt = (int)expFloat;
            SetExpAmount(expAmountInt);

            if (expAmountInt != _animateExpContext.ExpAmountInt)
            {
                _audioPlayer.PlaySound(SoundKey.ExpTick);
            }

            _animateExpContext.ExpAmountInt = expAmountInt;
        }

        private void SetExpAmount(int expAmount)
        {
            var targetExpAmount = _isLevelUp ? _currentLevelExpTarget : _nextLevelExpTarget;
            var expProgress = _isLevelUp
                ? (float)(expAmount - _prevLevelExpTarget) / (_currentLevelExpTarget - _prevLevelExpTarget)
                : (float)(expAmount - _currentLevelExpTarget) / (_nextLevelExpTarget - _currentLevelExpTarget);
            expProgress = Mathf.Clamp(expProgress, 0, 1);
            if (expProgress >= 1)
            {
                _expProgressLineIsFullAction?.Invoke();
                _expProgressLineIsFullAction = null;
            }
            
            _rootView.SetExpProgressLineXScale(expProgress);
            _rootView.SetExpAmountText($"{FormattingHelper.ToCommaSeparatedNumber(expAmount)} / {FormattingHelper.ToCommaSeparatedNumber(targetExpAmount)}" );
        }

        private async UniTaskVoid RunNewLevelFlow()
        {
            await Delay(800);
            _expProgressLineIsFullAction = ShowNewLevelStars;
            await LeanTween
                .value(_rootView.gameObject, SetExpAmount, _prevExpAmount, _playerModel.ExpAmount, 2f)
                .setEaseOutQuad()
                .ToUniTask();
            
            await Delay(300);
            await _rootView.AnimateCongratsTextAppear();
            
            await Delay(1000);
            await _rootView.AnimateNewCarsTextAppear();

            await AnimateCarIconsAppearing();
            
            await Delay(200);
            _rootView.AnimateContinueButtonAppear();
        }

        private void ShowNewLevelStars()
        {
            _rootView.ShowStarParticles();
            
            _audioPlayer.PlaySound(SoundKey.NewLevel);
        }

        private UniTask AnimateCarIconsAppearing()
        {
            var newCarsData = _carDataProvider.CarDataList
                .Where(c => c.UnlockLevel == _playerModel.Level)
                .ToArray();
            
            foreach (var carData in newCarsData)
            {
                _rootView.AddCarIcon(carData.IconSprite);
            }

            var animationTask = UniTask.CompletedTask;
            
            for (var i = 0; i < _rootView.CarIconViews.Count; i++)
            {
                var carIconView = _rootView.CarIconViews[i];
                animationTask = carIconView.AnimateFadeIn(i * 0.3f + 0.2f);
            }

            return animationTask;
        }

        private static UniTask Delay(int ms)
        {
            return UniTask.Delay(ms, ignoreTimeScale: true);
        }
        
        private class AnimateExpContext
        {
            public int ExpAmountInt;
        }
    }
}