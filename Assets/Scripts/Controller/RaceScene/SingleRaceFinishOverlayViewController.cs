using Data;
using Infra.Instance;
using Model;
using Model.RaceScene;
using Providers;
using UnityEngine;
using View.UI.RaceScene;
using static View.Helpers.RichTextHelper;

namespace Controller.RaceScene
{
    public class SingleRaceFinishOverlayViewController : RaceFinishOverlayViewControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        
        private RaceModel _raceModel;
        private RaceResultsModel _raceResultsModel;
        private UIRaceFinishOverlayView _finishOverlayView;
        private PlayerModel _playerModel;

        public SingleRaceFinishOverlayViewController(Transform targetTransform) : base(targetTransform)
        {
        }

        public override void Initialize()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            _raceModel = _modelsHolder.GetRaceModel();
            _raceResultsModel = _raceModel.RaceResultsModel;
            
            base.Initialize();
        }

        protected override UIRaceFinishOverlayViewBase InstantiateView()
        {
            _finishOverlayView = Instantiate<UIRaceFinishOverlayView>(PrefabKey.UIFinishOverlay, TargetTransform);
            
            return _finishOverlayView;
        }

        protected override void SetupView()
        {
            base.SetupView();
            
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
        }
    }
}