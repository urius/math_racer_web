using Cysharp.Threading.Tasks;
using Data;
using Holders;
using Holders.LocalizationProvider;
using Infra.Instance;
using Model;
using Model.RaceScene;
using UnityEngine;
using View.UI.RaceScene;
using static View.Helpers.RichTextHelper;

namespace Controller.RaceScene
{
    public class RaceFinishOverlayViewController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        private readonly Transform _targetTransform;
        private RaceModel _raceModel;
        private RaceResultsModel _raceResultsModel;
        private UIRaceFinishOverlayView _finishOverlayView;
        private PlayerModel _playerModel;

        public RaceFinishOverlayViewController(Transform targetTransform)
        {
            _targetTransform = targetTransform;
        }

        public override void Initialize()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            _raceModel = _modelsHolder.GetRaceModel();
            _raceResultsModel = _raceModel.RaceResultsModel;
            
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
                $"{GetLocale(LocalizationKeys.Complexity)}:\n{GetLocale(LocalizationKeys.Distance)}:\n \n{GetLocale(LocalizationKeys.Accelerations)}:\n{GetLocale(LocalizationKeys.Brakings)}:\n{GetLocale(LocalizationKeys.Speed)}:";
            _finishOverlayView.SetStatsKeysText(keysText);

            var complexityPercentStr = FormatGreen($"{_playerModel.GetOverallComplexityPercent()} %");
            var distanceStr = FormatGreen($"{_raceModel.DistanceMeters} {GetLocale(LocalizationKeys.MetersShort)}");
            var wrongAnswersText = _raceResultsModel.WrongAnswersCount <= 0
                ? FormatGreen(_raceResultsModel.WrongAnswersCount)
                : FormatRed(_raceResultsModel.WrongAnswersCount); 
            var valuesText =
                $"{complexityPercentStr}\n{distanceStr}\n \n{FormatGreen(_raceResultsModel.RightAnswersCount)}\n{wrongAnswersText}\n{FormatGreen(_raceResultsModel.PlayerSpeed)} {FormatGreen(GetLocale(LocalizationKeys.KmH))}";
            _finishOverlayView.SetStatsValuesText(valuesText);

            DisplayRewards();
            
            _finishOverlayView.DoubleRewardsButtonView.SetText($"{Constants.TextSpriteAds} {GetLocale(LocalizationKeys.DoubleTheRewardButton)}");
            _finishOverlayView.ContinueButtonView.SetText(GetLocale(LocalizationKeys.ContinueButton));
        }

        private void DisplayRewards()
        {
            var cashRewardAmountText = FormatGreen($"+{_raceModel.GetCashReward()}");
            var cashRewardText = $"{cashRewardAmountText} {Constants.TextSpriteCash}";

            var goldRewardAmount = _raceModel.GetGoldReward();
            var goldRewardAmountText = FormatColor($"+{_raceModel.GetGoldReward()}", "#0169FF");
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
            
        }

        private void OnDoubleRewardsClicked()
        {
            
        }

        private string GetLocale(string key)
        {
            return _localizationProvider.GetLocale(key);
        }
    }
}