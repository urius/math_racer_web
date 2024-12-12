using Data;
using Infra.Instance;
using Model;
using Model.RaceScene;
using Providers;
using View.Extensions;
using View.UI.RaceScene;

namespace Controller.RaceScene
{
    public class RaceSceneAnswerHintController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        
        private readonly UIAnswerHintView _answerHintView;
        
        private RaceModel _raceModel;
        private QuestionsModel _questionsModel;
        private PlayerModel _playerModel;

        public RaceSceneAnswerHintController(UIAnswerHintView answerHintView)
        {
            _answerHintView = answerHintView;
        }

        private bool CanBuyHint => _playerModel.GoldAmount >= Constants.AnswerHintCostCrystals;

        public override void Initialize()
        {
            _playerModel = _modelsHolder.GetPlayerModel();
            _raceModel = _modelsHolder.GetRaceModel();
            _questionsModel = _raceModel.QuestionsModel;

            UpdateCrystalsLeft();
            UpdateHintButtonInteractable();
            
            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void UpdateCrystalsLeft()
        {
            _answerHintView.SetCrystalsLeftText(_playerModel.GoldAmount.ToGoldView2());
        }

        private void UpdateHintButtonInteractable()
        {
            _answerHintView.SetHintButtonInteractable(CanBuyHint);
        }

        private void Subscribe()
        {
            _questionsModel.AnswerHintAvailableFlag.ValueChanged += OnAnswerHintAvailableFlagValueChanged;
        }

        private void Unsubscribe()
        {
            _questionsModel.AnswerHintAvailableFlag.ValueChanged -= OnAnswerHintAvailableFlagValueChanged;
        }

        private void OnAnswerHintAvailableFlagValueChanged(bool prevValue, bool currentValue)
        {
            if (currentValue)
            {
                if (CanBuyHint)
                {
                    _answerHintView.AnimateShowHint();
                }
            }
            else
            {
                _answerHintView.AnimateHideHint();
            }
        }
    }
}