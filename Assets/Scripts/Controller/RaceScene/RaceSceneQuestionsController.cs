using System.Text;
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

        private QuestionsModel _questionsModel;

        public RaceSceneQuestionsController(UIRightPanelView rightPanelView)
        {
            _rightPanelView = rightPanelView;
        }

        public override void Initialize()
        {
            _questionsModel = _modelsHolder.GetRaceModel().QuestionsModel;

            for (var i = 0; i < 100; i++)
            {
                TestGenerateExpressionMethod();
            }

            Subscribe();

            GenerateExpression();
            DisplayExpression();
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

        private ComplexityData GetComplexityData()
        {
            return _complexityDataProvider.GetComplexityData(15, 10);
        }

        private void GenerateExpression()
        {
            var complexity = GetComplexityData();
            _questionsModel.GenerateExpression(complexity);
        }

        private void DisplayExpression()
        {
            _rightPanelView.SetQuestionText(FormatExpression(_questionsModel.Expression));

            _rightPanelView.AnswersPanel.SetAnswersAmount(_questionsModel.Answers.Length);

            for (var i = 0; i < _questionsModel.Answers.Length; i++)
            {
                _rightPanelView.AnswersPanel.SetAnswerValue(i, _questionsModel.Answers[i]);
            }
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