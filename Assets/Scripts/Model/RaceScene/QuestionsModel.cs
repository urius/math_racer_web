using System;
using Data;
using Utils;

namespace Model.RaceScene
{
    public class QuestionsModel
    {
        public event Action RightAnswerGiven;
        public event Action WrongAnswerGiven;

        public readonly double[] Answers = new double[4];
        
        private readonly Random _random;

        private int _rightAnswerIndex = -1;

        public QuestionsModel()
        {
            _random = new Random();
        }

        public string Expression { get; private set; }

        public virtual void DispatchRightAnswerGiven()
        {
            RightAnswerGiven?.Invoke();
        }

        public virtual void DispatchWrongAnswerGiven()
        {
            WrongAnswerGiven?.Invoke();
        }

        public void GenerateExpression(ComplexityData complexityData)
        {
            Expression = ExpressionsHelper.GenerateExpression(complexityData);
            
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

        private static double ToFixed(double number)
        {
            return Math.Round(number, 2, MidpointRounding.AwayFromZero);
        }
    }
}