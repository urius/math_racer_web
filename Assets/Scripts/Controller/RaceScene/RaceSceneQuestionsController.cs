using System.Text;
using Cysharp.Threading.Tasks;
using Data;
using Holders;
using Infra.Instance;
using Model.RaceScene;
using UnityEngine;
using Utils;
using View.UI.RaceScene;

namespace Controller.RaceScene
{
    public class RaceSceneQuestionsController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IComplexityDataProvider _complexityDataProvider = Instance.Get<IComplexityDataProvider>();

        private readonly UIRightPanelView _rightPanelView;
        private readonly UIAnswersPanelView _answersPanel;

        private QuestionsModel _questionsModel;

        public RaceSceneQuestionsController(UIRightPanelView rightPanelView)
        {
            _rightPanelView = rightPanelView;
            _answersPanel = _rightPanelView.AnswersPanel;
        }

        public override void Initialize()
        {
            _questionsModel = _modelsHolder.GetRaceModel().QuestionsModel;

            for (var i = 0; i < 100; i++)
            {
                TestGenerateExpressionMethod();
            }

            Subscribe();

            RefreshQuestion();
        }

        private void TestGenerateExpressionMethod()
        {
            var complexityData = GetComplexityData();

            var expression = ExpressionsHelper.GenerateExpression(complexityData);
            var rightAnswer = ExpressionsHelper.EvaluateExpression(expression);

            Debug.Log("expression: " + expression + " = " + rightAnswer);
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _answersPanel.AnswerClicked += OnAnswerClicked;
        }

        private void Unsubscribe()
        {
            _answersPanel.AnswerClicked -= OnAnswerClicked;
        }

        private void OnAnswerClicked(int answerIndex)
        {
            var isCorrectAnswer = _questionsModel.GiveAnswer(answerIndex);
            
            _answersPanel.SetAnswerInteractable(answerIndex, false);

            var answerView = _answersPanel.AnswerViews[answerIndex];

            if (isCorrectAnswer)
            {
                answerView.ToRightAnswerState();

                ProcessNextQuestion().Forget();
            }
            else
            {
                answerView.ToWrongAnswerState();
            }
        }

        private async UniTaskVoid ProcessNextQuestion()
        {
            foreach (var answerView in _answersPanel.AnswerViews)
            {
                answerView.SetInteractable(false);
            }

            await UniTask.Delay(1000);
            
            RefreshQuestion();
        }

        private ComplexityData GetComplexityData()
        {
            return _complexityDataProvider.GetComplexityData(15, 10);
        }

        private void GenerateQuestion()
        {
            _questionsModel.GenerateQuestion();
        }

        private void DisplayQuestion()
        {
            _rightPanelView.SetQuestionText(FormatExpression(_questionsModel.Expression));

            _answersPanel.SetAnswersAmount(_questionsModel.Answers.Length);

            for (var i = 0; i < _questionsModel.Answers.Length; i++)
            {
                _answersPanel.SetAnswerValue(i, _questionsModel.Answers[i]);
            }
        }

        private void RefreshQuestion()
        {
            GenerateQuestion();
            DisplayQuestion();
        }

        private static string FormatExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return expression;
            }

            var sb = new StringBuilder(expression.Length);

            foreach (var c in expression)
            {
                switch (c)
                {
                    case Constants.OperatorMultiplyChar:
                        sb.Append(" x ");
                        break;
                    case Constants.OperatorDivideChar:
                        sb.Append(" : ");
                        break;
                    case Constants.OperatorPlusChar:
                        sb.Append(" + ");
                        break;
                    case Constants.OperatorMinusChar:
                        sb.Append(" - ");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            // Trim any leading or trailing spaces
            return sb.ToString().Trim();
        }
    }
}