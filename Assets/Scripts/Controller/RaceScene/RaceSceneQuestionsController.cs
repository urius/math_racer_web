using Holders;
using Infra.Instance;
using Model.RaceScene;
using View.UI.RaceScene;

namespace Controller.RaceScene
{
    public class RaceSceneQuestionsController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        
        private readonly UIRightPanelView _rightPanelView;
        
        private QuestionsModel _questionsModel;

        public RaceSceneQuestionsController(UIRightPanelView rightPanelView)
        {
            _rightPanelView = rightPanelView;
        }

        public override void Initialize()
        {
            _questionsModel = _modelsHolder.GetRaceModel().QuestionsModel;

            Subscribe();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _rightPanelView.AnswersPanel.AnswerClicked += OnAnswerClicked;
        }

        private void Unsubscribe()
        {
            _rightPanelView.AnswersPanel.AnswerClicked -= OnAnswerClicked;
        }

        private void OnAnswerClicked(int answerIndex)
        {
            if (answerIndex == 0)
            {
                _questionsModel.DispatchRightAnswerGiven();
            }
            else
            {
                _questionsModel.DispatchWrongAnswerGiven();
            }
        }
    }
}