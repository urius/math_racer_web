using Cysharp.Threading.Tasks;
using Data;
using Events;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.RaceScene;
using Providers;
using Providers.LocalizationProvider;
using UnityEngine;
using View.UI.RaceScene;
using static View.Helpers.RichTextHelper;

namespace Controller.RaceScene
{
    public class RaceFinishOverlayViewController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        
        private readonly Transform _targetTransform;
        
        private RaceModel _raceModel;
        private RaceResultsModel _raceResultsModel;
        private UIRaceFinishOverlayView _finishOverlayView;
        private PlayerModel _playerModel;
        private RaceRewardsModel _raceRewards;

        public RaceFinishOverlayViewController(Transform targetTransform)
        {
            _targetTransform = targetTransform;
        }

        public override void Initialize()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            _raceModel = _modelsHolder.GetRaceModel();
            _raceResultsModel = _raceModel.RaceResultsModel;
            _raceRewards = _raceModel.RaceRewards; 
            
            InitView().Forget();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private async UniTaskVoid InitView()
        {
            _finishOverlayView = Instantiate<UIRaceFinishOverlayView>(PrefabKey.UIFinishOverlay, _targetTransform);
            
            SetupView();
            
            await _finishOverlayView.AnimateShow(_raceResultsModel.IsFirst);

            Subscribe();
        }

        private void SetupView()
        {
            _finishOverlayView.SetFinishText(GetLocale(LocalizationKeys.Finish));
            _finishOverlayView.SetPlaceText(GetLocale(LocalizationKeys.FirstPlace));
            
            var keysText =
                $"{GetLocale(LocalizationKeys.Complexity)}:\n{GetLocale(LocalizationKeys.Distance)}:\n \n{GetLocale(LocalizationKeys.Accelerations)}:\n{GetLocale(LocalizationKeys.TurboBoosts)}:\n{GetLocale(LocalizationKeys.Brakings)}:\n{GetLocale(LocalizationKeys.Speed)}:";
            _finishOverlayView.SetStatsKeysText(keysText);

            var complexityPercentStr = FormatGreen($"{_playerModel.GetOverallComplexityPercent()} %");
            var distanceStr = FormatGreen($"{_raceModel.DistanceMeters} {GetLocale(LocalizationKeys.MetersShort)}");
            var turboBoostsText = _raceResultsModel.TurboBoostsCount <= 0
                ? _raceResultsModel.TurboBoostsCount.ToString()
                : FormatGreen($"{_raceResultsModel.TurboBoostsCount}");
            var wrongAnswersText = _raceResultsModel.WrongAnswersCount <= 0
                ? FormatGreen(_raceResultsModel.WrongAnswersCount)
                : FormatRed(_raceResultsModel.WrongAnswersCount); 
            var valuesText =
                $"{complexityPercentStr}\n{distanceStr}\n \n{FormatGreen(_raceResultsModel.RightAnswersCount)}\n{turboBoostsText}\n{wrongAnswersText}\n{FormatGreen(_raceResultsModel.PlayerSpeed)} {FormatGreen(GetLocale(LocalizationKeys.KmH))}";
            _finishOverlayView.SetStatsValuesText(valuesText);

            DisplayRewards();
            
            _finishOverlayView.DoubleRewardsButtonView.SetText($"{Constants.TextSpriteAds} {GetLocale(LocalizationKeys.DoubleTheRewardButton)}");
            _finishOverlayView.ContinueButtonView.SetText(GetLocale(LocalizationKeys.ContinueButton));
        }

        private void DisplayRewards()
        {
            var cashRewardAmountText = FormatGreen($"+{_raceRewards.CashReward}");
            var cashRewardText = $"{cashRewardAmountText} {Constants.TextSpriteCash}";

            var goldRewardAmount = _raceRewards.GoldReward;
            var goldRewardAmountText = FormatColor($"+{_raceRewards.GoldReward}", Constants.TextCrystalBlueColor);
            var goldRewardText = goldRewardAmount > 0
                ? $"{goldRewardAmountText} {Constants.TextSpriteCrystal}"
                : string.Empty;

            _finishOverlayView.SetRewardTexts($"{cashRewardText}  {goldRewardText}");
        }

        private void Subscribe()
        {
            _finishOverlayView.ContinueButtonView.ButtonClicked += OnContinueClicked;
            _finishOverlayView.DoubleRewardsButtonView.ButtonClicked += OnDoubleRewardsClicked;
        }

        private void Unsubscribe()
        {
            _finishOverlayView.ContinueButtonView.ButtonClicked -= OnContinueClicked;
            _finishOverlayView.DoubleRewardsButtonView.ButtonClicked -= OnDoubleRewardsClicked;
        }

        private void OnContinueClicked()
        {
            ProcessUpdateComplexity();
            var takeRewardsResult = ProcessTakingRewards();

            _eventBus.Dispatch(takeRewardsResult == ProcessTakingRewardsResult.LevelUp
                ? new RequestNextSceneEvent(Constants.NewLevelSceneName)
                : new RequestNextSceneEvent());
        }

        private void ProcessUpdateComplexity()
        {
            var rndValue = Random.value;
            var totalAnswersCount = _raceResultsModel.RightAnswersCount + _raceResultsModel.WrongAnswersCount;

            var downThreshold = (float)_raceResultsModel.WrongAnswersCount / totalAnswersCount;
            var upThreshold = (float)_raceResultsModel.RightAnswersCount / totalAnswersCount - downThreshold;

            if (rndValue <= upThreshold)
            {
                _playerModel.IncreaseComplexityLevel();
                Debug.Log($"Complexity increased to level {_playerModel.ComplexityLevel}, upThreshold: {upThreshold}, rndValue: {rndValue}");
            }

            if (rndValue <= downThreshold)
            {
                _playerModel.DecreaseComplexityLevel();
                Debug.Log($"Complexity decreased to level {_playerModel.ComplexityLevel}, downThreshold: {downThreshold}, rndValue: {rndValue}");
            }
        }

        private ProcessTakingRewardsResult ProcessTakingRewards()
        {
            var levelBeforeTaking = _playerModel.Level;
            
            _playerModel.AddCash(_raceRewards.CashReward);
            _playerModel.AddGold(_raceRewards.GoldReward);
            _playerModel.AddExpAmount(_raceRewards.ExpReward);

            var levelAfterTaking = _playerModel.Level;

            //return ProcessTakingRewardsResult.LevelUp; //temp
            
            return levelAfterTaking > levelBeforeTaking ? ProcessTakingRewardsResult.LevelUp : ProcessTakingRewardsResult.Default;
        }

        private void OnDoubleRewardsClicked()
        {
            
        }

        private string GetLocale(string key)
        {
            return _localizationProvider.GetLocale(key);
        }
        
        private enum ProcessTakingRewardsResult
        {
            Default,
            LevelUp,
        }
    }
}