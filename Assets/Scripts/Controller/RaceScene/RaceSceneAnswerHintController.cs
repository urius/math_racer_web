using Data;
using Infra.Instance;
using Model;
using Model.RaceScene;
using Providers;
using Providers.LocalizationProvider;
using View.Extensions;
using View.UI.RaceScene;

namespace Controller.RaceScene
{
    public class RaceSceneAnswerHintController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();
        
        private readonly UIAnswerHintView _answerHintView;
        
        private RaceModel _raceModel;
        private QuestionsModel _questionsModel;
        private PlayerModel _playerModel;

        public RaceSceneAnswerHintController(UIAnswerHintView answerHintView)
        {
            _answerHintView = answerHintView;
        }

        private bool CanBuyHint => _playerModel.GoldAmount >= Constants.SolveCostCrystals;

        public override void Initialize()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            _raceModel = _modelsHolder.GetRaceModel();
            _questionsModel = _raceModel.QuestionsModel;

            SetHintButtonText();
            UpdateCrystalsLeft();
            UpdateHintButtonInteractable();
            
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void SetHintButtonText()
        {
            var solveText = _localizationProvider.GetLocale(LocalizationKeys.SolveButtonText);
            _answerHintView.SetHintButtonText($"{solveText} ({Constants.SolveCostCrystals.ToGoldView(Constants.TextCrystalBlueColor)})");
        }

        private void UpdateCrystalsLeft()
        {
            _answerHintView.SetCrystalsLeftText(_playerModel.GoldAmount.ToGoldView2());
        }

        private void UpdateHintButtonInteractable()
        {
            _answerHintView.SetHintButtonInteractable(CanBuyHint && _questionsModel.IsRightAnswerGiven == false);
        }

        private void Subscribe()
        {
            _playerModel.GoldAmountChanged += OnGoldAmountChanged;
            _questionsModel.AnswerGiven += OnAnswerGiven;
            _questionsModel.AnswerHintAvailableFlag.ValueChanged += OnAnswerHintAvailableFlagValueChanged;
            _answerHintView.HintButtonClicked += OnHintButtonClicked;
        }

        private void Unsubscribe()
        {
            _playerModel.GoldAmountChanged -= OnGoldAmountChanged;
            _questionsModel.AnswerGiven -= OnAnswerGiven;
            _questionsModel.AnswerHintAvailableFlag.ValueChanged -= OnAnswerHintAvailableFlagValueChanged;
            _answerHintView.HintButtonClicked -= OnHintButtonClicked;
        }

        private void OnGoldAmountChanged(int _)
        {
            UpdateCrystalsLeft();
        }

        private void OnAnswerGiven(int _, bool __)
        {
            UpdateHintButtonInteractable();
        }

        private void OnHintButtonClicked()
        {
            if (_playerModel.TrySpendGold(Constants.SolveCostCrystals))
            {
                _questionsModel.GiveRightAnswer();
            }
        }

        private void OnAnswerHintAvailableFlagValueChanged(bool prevValue, bool currentValue)
        {
            if (currentValue)
            {
                if (CanBuyHint)
                {
                    _answerHintView.AnimateShowHint();
                    UpdateHintButtonInteractable();
                }
            }
            else
            {
                _answerHintView.AnimateHideHint();
            }
        }
    }
}