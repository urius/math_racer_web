using System;
using Data;
using Utils;

namespace Model.RaceScene
{
    public class QuestionsModel
    {
        public event Action<bool> AnswerGiven;

        public readonly double[] Answers = new double[4];

        private readonly Random _random;
        private readonly ComplexityData _complexityData;

        private int _rightAnswerIndex = -1;

        public QuestionsModel(ComplexityData complexityData)
        {
            _random = new Random();
            
            _complexityData = complexityData;
        }

        public string Expression { get; private set; }
        public bool IsRightAnswerGiven { get; private set; }

        public void GenerateQuestion()
        {
            ResetAnswersData();
            
            Expression = ExpressionsHelper.GenerateExpression(_complexityData);
            
            var rightAnswer = ExpressionsHelper.EvaluateExpression(Expression);

            _rightAnswerIndex = _random.Next(0, Answers.Length);

            Answers[_rightAnswerIndex] = ToFixed(rightAnswer);

            var answerOffset = (double)_random.Next(1, Math.Max(3, (int)Math.Abs(rightAnswer * 0.5f)));
            
            if (rightAnswer != (int)rightAnswer)
            {
                answerOffset += _random.NextDouble();
            }

            for (var i = 0; i < Answers.Length; i++)
            {
                if (i != _rightAnswerIndex)
                {
                    var wrongAnswer = rightAnswer + (i - _rightAnswerIndex) * answerOffset;
                    Answers[i] = ToFixed(wrongAnswer);
                }
            }
        }

        private void ResetAnswersData()
        {
            _rightAnswerIndex = -1;
            IsRightAnswerGiven = false;
        }

        public bool GiveAnswer(int answerIndex)
        {
            IsRightAnswerGiven = answerIndex == _rightAnswerIndex;
            
            AnswerGiven?.Invoke(IsRightAnswerGiven);
            
            return IsRightAnswerGiven;
        }

        private static double ToFixed(double number)
        {
            return Math.Round(number, 2, MidpointRounding.AwayFromZero);
        }
    }
}