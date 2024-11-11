using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Holders;
using Infra.Instance;
using Model.RaceScene;
using UnityEngine;
using View.UI.RaceScene;
using Random = System.Random;

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

            for (int i = 0; i < 100; i++)
            {
                TestGenerateExpressionMethod();
            }

            Subscribe();
        }

        private void TestGenerateExpressionMethod()
        {
            var expression = GenerateExpression(
                new Random().Next(2, 7), 
                new[] { "+", "-", "*" },
                10);
            var rightAnswer = Evaluate(expression);

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

        private static string GenerateExpression(int numbersCount, IReadOnlyList<string> availableOperators, int maxNumberValue)
        {
            if (numbersCount < 2 || availableOperators == null || availableOperators.Count == 0)
            {
                throw new ArgumentException("Invalid input parameters.");
            }

            var random = new Random();
            var expression = new StringBuilder();
            var openBracesCounter = 0;

            for (var i = 0; i < numbersCount; i++)
            {
                expression.Append(random.Next(1, maxNumberValue));
                if (openBracesCounter > 0)
                {
                    openBracesCounter--;
                    if (openBracesCounter <= 0)
                    {
                        expression.Append(")");
                    }
                }

                if (i < numbersCount - 1)
                {
                    var randomOperator = availableOperators[random.Next(availableOperators.Count)];
                    expression.Append(randomOperator);

                    if (randomOperator == "*"
                        && numbersCount - i > 2
                        && openBracesCounter <= 0
                        && random.NextDouble() < 0.9f)
                    {
                        openBracesCounter = random.Next(2, numbersCount - i);
                        expression.Append("(");
                    }
                }
            }

            return expression.ToString();
        }

        private static double Evaluate(string expression)
        {
            var dataTable = new DataTable();
            var result = dataTable.Compute(expression, string.Empty);
            return Convert.ToDouble(result);
        }
    }
}