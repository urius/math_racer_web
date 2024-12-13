using System;
using Data;
using Utils;
using Utils.ReactiveValue;

namespace Model.RaceScene
{
    public class QuestionsModel
    {
        private const int DefaultTurboTime = 10;
        private const int TurboTimeMin = 1;
        private const int TurboIndicatorsMax = 5;
        
        public event Action<int, bool> AnswerGiven;
        public event Action TurboActivated;
        public event Action<int> TurboIndicatorAdded;
        public event Action<int> TurboIndicatorRemoved;

        public readonly double[] Answers = new double[4];
        public readonly ReactiveFlag AnswerHintAvailableFlag = new(initialValue: false);

        private readonly Random _random;
        private readonly ComplexityData _complexityData;

        private int _rightAnswerIndex = -1;
        private string _prevExpression = null;

        public QuestionsModel(ComplexityData complexityData)
        {
            _random = new Random();
            
            _complexityData = complexityData;
        }

        public string Expression { get; private set; }
        public bool IsRightAnswerGiven { get; private set; }
        public bool IsAnswerGiven { get; private set; }
        public int QuestionsCount { get; private set; } = 0;
        public int RightAnswersCountTotal { get; private set; } = 0;
        public int WrongAnswersCountTotal { get; private set; } = 0;
        public int WrongAnswersCountForQuestion { get; private set; } = 0;
        public int TurboBoostIndicatorsCount { get; private set; } = 0;
        public float TurboTimeInitial { get; private set; } = 0;
        public float TurboTimeLeft { get; private set; } = 0;
        public int TurboBoostsCount { get; private set; } = 0;
        public bool IsFirstQuestion => QuestionsCount <= 1;

        public void GenerateQuestion()
        {
            _prevExpression = Expression;
            
            ResetAnswersData();
            
            QuestionsCount++;

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

        public void GiveRightAnswer()
        {
            GiveAnswer(_rightAnswerIndex);
        }

        public void GiveAnswer(int answerIndex)
        {
            IsAnswerGiven = true;
            IsRightAnswerGiven = answerIndex == _rightAnswerIndex;

            if (IsRightAnswerGiven)
            {
                RightAnswersCountTotal++;
            }
            else
            {
                WrongAnswersCountTotal++;
                WrongAnswersCountForQuestion++;
                TurboTimeLeft = 0;
                DecrementTurboIndicators();
            }

            if (IsRightAnswerGiven
                && (TurboTimeLeft > 0 || IsFirstQuestion)
                && WrongAnswersCountForQuestion <= 0)
            {
                IncrementTurboIndicators();
            }

            AnswerGiven?.Invoke(answerIndex, IsRightAnswerGiven);
        }

        private void InitTurboTime()
        {
            if (QuestionsCount <= 1)
            {
                TurboTimeInitial = TurboTimeLeft = 0;
                return;
            }

            TurboTimeInitial = TurboTimeLeft = Math.Max(TurboTimeMin, DefaultTurboTime);// - Math.Max(0, TurboBoostIndicatorsCount));
        }

        private void UpdateTurboTime(float deltaTime)
        {
            if (IsRightAnswerGiven || TurboTimeLeft <= 0) return;
            
            TurboTimeLeft -= deltaTime;
            if (TurboTimeLeft <= 0)
            {
                DecrementTurboIndicators();
                if (TurboBoostIndicatorsCount > 0)
                {
                    InitTurboTime();
                }
            }
            else if (AnswerHintAvailableFlag.Value == false 
                     && TurboTimeLeft <= TurboTimeInitial / 2)
            {
                AnswerHintAvailableFlag.Value = true;
            }
        }

        private void IncrementTurboIndicators()
        {
            if (TurboBoostIndicatorsCount < TurboIndicatorsMax)
            {
                TurboBoostIndicatorsCount++;
                var index = TurboBoostIndicatorsCount - 1;
                TurboIndicatorAdded?.Invoke(index);

                if (TurboBoostIndicatorsCount >= TurboIndicatorsMax)
                {
                    TurboBoostsCount++;
                    
                    TurboActivated?.Invoke();
                    TurboBoostIndicatorsCount = 0;
                }
            }
        }
        
        private void DecrementTurboIndicators()
        {
            if (TurboBoostIndicatorsCount > 0)
            {
                TurboBoostIndicatorsCount--;
                var index = TurboBoostIndicatorsCount;
                TurboIndicatorRemoved?.Invoke(index);
            }
        }

        private void ResetAnswersData()
        {
            _rightAnswerIndex = -1;
            WrongAnswersCountForQuestion = 0;
            IsRightAnswerGiven = IsAnswerGiven = false;
            Expression = null;
            AnswerHintAvailableFlag.Value = false;
        }

        private static double ToFixed(double number)
        {
            return Math.Round(number, 2, MidpointRounding.AwayFromZero);
        }
    }
}