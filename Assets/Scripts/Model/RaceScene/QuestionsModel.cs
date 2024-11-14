using System;
using Data;
using Utils;

namespace Model.RaceScene
{
    public class QuestionsModel
    {
        private const int DefaultTurboTime = 10;
        private const int TurboTimeMin = 1;
        
        public event Action<bool> AnswerGiven;

        public readonly double[] Answers = new double[4];

        private readonly Random _random;
        private readonly ComplexityData _complexityData;

        private int _rightAnswerIndex = -1;
        private string _prevExpression = null;
        private int _wrongAnswersCount = 0;

        public QuestionsModel(ComplexityData complexityData)
        {
            _random = new Random();
            
            _complexityData = complexityData;
        }

        public string Expression { get; private set; }
        public bool IsRightAnswerGiven { get; private set; }
        public int TurboLevel { get; private set; } = 0;
        public float TurboTimeInitial { get; private set; } = 0;
        public float TurboTimeLeft { get; private set; } = 0;

        public void GenerateQuestion()
        {
            _prevExpression = Expression;
            
            ResetAnswersData();

            while (Expression == null || Expression == _prevExpression)
            {
                Expression = ExpressionsHelper.GenerateExpression(_complexityData);
            }
            
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

            InitTurboTime();
        }

        public void Update(float deltaTime)
        {
            UpdateTurboTime(deltaTime);
        }

        public bool GiveAnswer(int answerIndex)
        {
            IsRightAnswerGiven = answerIndex == _rightAnswerIndex;

            if (IsRightAnswerGiven == false)
            {
                _wrongAnswersCount++;
                TurboTimeLeft = 0;
            }

            if (IsRightAnswerGiven && TurboTimeLeft > 0 && _wrongAnswersCount <= 0)
            {
                TurboLevel++;
            }
            else
            {
                TurboLevel = 0;
            }
            
            AnswerGiven?.Invoke(IsRightAnswerGiven);
            
            return IsRightAnswerGiven;
        }

        private void InitTurboTime()
        {
            TurboTimeInitial = TurboTimeLeft = Math.Max(TurboTimeMin, DefaultTurboTime - Math.Max(0, TurboLevel));
        }

        private void UpdateTurboTime(float deltaTime)
        {
            if (IsRightAnswerGiven) return;
            
            TurboTimeLeft -= deltaTime;
            if (TurboTimeLeft <= 0)
            {
                TurboTimeLeft = 0;
                TurboLevel = 0;
            }
        }

        private void ResetAnswersData()
        {
            _rightAnswerIndex = -1;
            _wrongAnswersCount = 0;
            IsRightAnswerGiven = false;
            Expression = null;
        }

        private static double ToFixed(double number)
        {
            return Math.Round(number, 2, MidpointRounding.AwayFromZero);
        }
    }
}