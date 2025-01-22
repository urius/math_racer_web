using Cysharp.Threading.Tasks;
using Data;
using Events;
using Extensions;
using Infra.EventBus;
using Infra.Instance;
using Model;
using Model.RaceScene;
using Providers;
using Providers.LocalizationProvider;
using UnityEngine;
using Utils.AudioManager;
using View.UI.RaceScene;
using static View.Helpers.RichTextHelper;

namespace Controller.RaceScene
{
    public abstract class RaceFinishOverlayViewControllerBase : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        private readonly IEventBus _eventBus = Instance.Get<IEventBus>();
        private readonly IAudioPlayer _audioPlayer = Instance.Get<IAudioPlayer>();

        private RaceModel _raceModel;
        private RaceResultsModel _raceResultsModel;
        private UIRaceFinishOverlayViewBase _finishOverlayView;
        private PlayerModel _playerModel;
        private RaceRewardsModel _raceRewards;

        protected Transform TargetTransform { get; }

        protected RaceFinishOverlayViewControllerBase(Transform targetTransform)
        {
            TargetTransform = targetTransform;
        }

        protected abstract UIRaceFinishOverlayViewBase InstantiateView();

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
            _finishOverlayView = InstantiateView();
            
            SetupView();

            _eventBus.Dispatch(new RequestFinishMusicEvent());

            await UniTask.Delay(1000);
            await _finishOverlayView.AnimateShow(_raceResultsModel.IsFirst);

            Subscribe();
        }

        protected virtual void SetupView()
        {
            _finishOverlayView.SetFinishText(GetLocale(LocalizationKeys.Finish));
            _finishOverlayView.SetPlaceText(GetLocale(LocalizationKeys.FirstPlace));

            DisplayRewards();
            
            _finishOverlayView.DoubleRewardsButtonView.SetText($"{Constants.TextSpriteAds} {GetLocale(LocalizationKeys.DoubleTheRewardButton)}");
            _finishOverlayView.ContinueButtonView.SetText(GetLocale(LocalizationKeys.ContinueButton));
        }

        protected string GetLocale(string key)
        {
            return _localizationProvider.GetLocale(key);
        }

        protected void DisplayRewards()
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
            
            _audioPlayer.PlayButtonSound();
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
            //show ads
            _audioPlayer.PlayButtonSound();
        }

        private enum ProcessTakingRewardsResult
        {
            Default,
            LevelUp,
        }
    }
}