using System.Linq;
using Data;
using Infra.Instance;
using Model.RaceScene;
using Providers;
using Providers.LocalizationProvider;
using Services;
using UnityEngine;
using View.UI.RaceScene;

namespace Controller.RaceScene
{
    public class NetRaceFinishOverlayViewController : RaceFinishOverlayViewControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IP2PRoomService _roomService = Instance.Get<IP2PRoomService>();
        
        private UINetRaceFinishOverlayView _finishOverlayView;
        private NetRaceModel _netRaceModel;

        public NetRaceFinishOverlayViewController(Transform targetTransform) : base(targetTransform)
        {
        }

        public override void Initialize()
        {
            _netRaceModel = (NetRaceModel)_modelsHolder.GetRaceModel();
            
            base.Initialize();
        }

        protected override void Subscribe()
        {
            base.Subscribe();

            _roomService.OpponentFinishedReceived += OnOpponentFinishedReceived;
        }

        protected override void Unsubscribe()
        {
            _roomService.OpponentFinishedReceived -= OnOpponentFinishedReceived;
            
            base.Unsubscribe();
        }

        protected override UIRaceFinishOverlayViewBase InstantiateView()
        {
            _finishOverlayView = Instantiate<UINetRaceFinishOverlayView>(PrefabKey.UINetFinishOverlay, TargetTransform);

            return _finishOverlayView;
        }

        protected override void SetupView()
        {
            base.SetupView();

            UpdateResultViews();
        }

        private void OnOpponentFinishedReceived(OpponentFinishedReceivedEventPayload payload)
        {
            UpdateResultViews();
        }

        private void UpdateResultViews()
        {
            var selfResultsData = ResultViewItemData.FromSelfRaceResults("You", _netRaceModel.RaceResultsModel);

            var allResultsData =
                _netRaceModel.NetOpponentRaceResults
                    .Select(pair => ResultViewItemData.FromOpponentRaceResults("Opponent " + pair.Key, pair.Value))
                    .Concat(new[] { selfResultsData })
                    .OrderBy(d => d.RaceTimeMs)
                    .ToArray();

            var maxSpeed = 0;
            var maxRightAnswers = 0;
            var minTimeMs = int.MaxValue;
            foreach (var data in allResultsData)
            {
                minTimeMs = data.RaceTimeMs < minTimeMs ? data.RaceTimeMs : minTimeMs;
                maxSpeed = data.Speed > maxSpeed ? data.Speed : maxSpeed;
                maxRightAnswers = data.RightAnswers > maxRightAnswers ? data.RightAnswers : maxRightAnswers;
            }

            var kmhText = _localizationProvider.GetLocale(LocalizationKeys.KmH);

            for (var i = 0; i < allResultsData.Length; i++)
            {
                var resultData = allResultsData[i];
                var resultView = _finishOverlayView.RaceResultItems[i];
                resultView.SetVisible(true);
                resultView.SetPlaceAndName(i + 1, resultData.Name, resultData.IsSelfResults);
                resultView.SetTimeText(resultData.RaceTimeMs, resultData.RaceTimeMs <= minTimeMs);
                resultView.SetSpeedText($"{resultData.Speed} {kmhText}", resultData.Speed >= maxSpeed);
                resultView.SetAnswers(resultData.RightAnswers, resultData.WrongAnswers,
                    resultData.RightAnswers >= maxRightAnswers);
            }
        }

        private struct ResultViewItemData
        {
            public readonly string Name;
            public readonly int RaceTimeMs;
            public readonly int Speed;
            public readonly int RightAnswers;
            public readonly int WrongAnswers;
            public readonly bool IsSelfResults;

            private ResultViewItemData(string name, int raceTimeMs, int speed, int rightAnswers, int wrongAnswers, bool isSelfResults)
            {
                Name = name;
                RaceTimeMs = raceTimeMs;
                Speed = speed;
                RightAnswers = rightAnswers;
                WrongAnswers = wrongAnswers;
                IsSelfResults = isSelfResults;
            }

            public static ResultViewItemData FromSelfRaceResults(string name, RaceResultsModel resultsModel)
            {
                return new ResultViewItemData(
                    name,
                    (int)(resultsModel.RaceTimeSec * 1000),
                    resultsModel.PlayerSpeed,
                    resultsModel.RightAnswersCount,
                    resultsModel.WrongAnswersCount,
                    isSelfResults: true);
            }
            
            public static ResultViewItemData FromOpponentRaceResults(string name, NetOpponentRaceResult opponentRaceResult)
            {
                return new ResultViewItemData(
                    name,
                    opponentRaceResult.RaceTimeMs,
                    opponentRaceResult.Speed,
                    opponentRaceResult.RightAnswersCount,
                    opponentRaceResult.WrongAnswersCount,
                    isSelfResults: false);
            }
        }
    }
}