using System.Text;
using Cysharp.Threading.Tasks;
using Data;
using Infra.Instance;
using Model.RaceScene;
using Providers;
using Providers.LocalizationProvider;
using UnityEngine;
using Utils;
using View.UI.RaceScene;

namespace Controller.RaceScene
{
    public class RaceSceneQuestionsController : ControllerBase
    {
        private readonly IModelsHolder _modelsHolder = Instance.Get<IModelsHolder>();
        private readonly IComplexityDataProvider _complexityDataProvider = Instance.Get<IComplexityDataProvider>();
        private readonly IUpdatesProvider _updatesProvider = Instance.Get<IUpdatesProvider>();
        private readonly ILocalizationProvider _localizationProvider = Instance.Get<ILocalizationProvider>();

        private readonly UIRightPanelView _rightPanelView;
        private readonly UIAnswersPanelView _answersPanel;
        private readonly UITurboTextView _turboTextView;

        private QuestionsModel _questionsModel;
        private RaceModel _raceModel;

        public RaceSceneQuestionsController(UIRightPanelView rightPanelView)
        {
            _rightPanelView = rightPanelView;
            _answersPanel = _rightPanelView.AnswersPanel;
            _turboTextView = _rightPanelView.TurboTextView;
        }

        public override void Initialize()
        {
            _raceModel = _modelsHolder.GetRaceModel();
            _questionsModel = _modelsHolder.GetRaceModel().QuestionsModel;

            for (var i = 0; i < 100; i++)
            {
                TestGenerateExpressionMethod();
            }

            Subscribe();

            RefreshQuestion();
            SetupTurboBoostText();
            UpdateTurboTextVisibility();
            
            InitChildControllers();
            
            _rightPanelView.AnimateShow();
        }

        public override void DisposeInternal()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _answersPanel.AnswerClicked += OnAnswerClicked;
            _updatesProvider.GameplayUpdate += OnGameplayUpdate;
            _raceModel.IsFinishingFlagChanged += OnIsFinishingFlagChanged;
            _questionsModel.TurboIndicatorAdded += OnTurboIndicatorAdded;
            _questionsModel.TurboIndicatorRemoved += OnTurboIndicatorRemoved;
            _questionsModel.TurboActivated += OnTurboActivated;
            _questionsModel.AnswerGiven += OnAnswerGiven;
        }

        private void Unsubscribe()
        {
            _answersPanel.AnswerClicked -= OnAnswerClicked;
            _updatesProvider.GameplayUpdate -= OnGameplayUpdate;
            _raceModel.IsFinishingFlagChanged -= OnIsFinishingFlagChanged;
            _questionsModel.TurboIndicatorAdded -= OnTurboIndicatorAdded;
            _questionsModel.TurboIndicatorRemoved -= OnTurboIndicatorRemoved;
            _questionsModel.TurboActivated -= OnTurboActivated;
            _questionsModel.AnswerGiven -= OnAnswerGiven;
        }

        private void InitChildControllers()
        {
            InitChildController(new RaceSceneAnswerHintController(_rightPanelView.AnswerHintView));
        }

        private void TestGenerateExpressionMethod()
        {
#if UNITY_EDITOR
            var complexityData = GetComplexityData();

            var expression = ExpressionsHelper.GenerateExpression(complexityData);
            var rightAnswer = ExpressionsHelper.EvaluateExpression(expression);

            Debug.Log("expression: " + expression + " = " + rightAnswer);
#endif
        }

        private void OnTurboIndicatorAdded(int index)
        {
            _rightPanelView.ShowGreenIndicator(index);
        }

        private void OnTurboIndicatorRemoved(int index)
        {
            _rightPanelView.HideRedIndicator(index);
        }

        private void OnTurboActivated()
        {
            ProcessTurboLogic().Forget();
        }

        private async UniTask ProcessTurboLogic()
        {
            await UniTask.Delay(Constants.TurboIndicatorShowHideDurationMs);
            
            _turboTextView.SetTextVisibility(true);
            
            await _rightPanelView.AnimateTurboBoost();

            await UniTask.Delay(2500);
            
            _turboTextView.SetTextVisibility(false);

            UpdateIndicatorViews();
        }

        private void UpdateIndicatorViews()
        {
            _rightPanelView.SetEnabledIndicators(_questionsModel.TurboBoostIndicatorsCount);
        }

        private void OnGameplayUpdate()
        {
            UpdateTurboLineView();
            UpdateTurboTextAlpha();
        }

        private void UpdateTurboTextAlpha()
        {
            var alpha = Mathf.PingPong(Time.realtimeSinceStartup, 0.5f) + 0.7f;
            _turboTextView.SetTextAlpha(alpha);
        }

        private void UpdateTurboLineView()
        {
            if (_questionsModel.TurboTimeInitial <= 0)
            {
                _rightPanelView.SetTurboTimerLineXScale(0);
            }
            else
            {
                _rightPanelView.SetTurboTimerLineXScale(_questionsModel.TurboTimeLeft / _questionsModel.TurboTimeInitial);
            }
        }

        private void OnAnswerClicked(int answerIndex)
        {
            _questionsModel.GiveAnswer(answerIndex);
        }

        private void OnAnswerGiven(int answerIndex, bool _)
        {
            HandleAnswer(answerIndex);
        }

        private void HandleAnswer(int answerIndex)
        {
            var isCorrectAnswer = _questionsModel.IsRightAnswerGiven;

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

            UpdateTurboLineColor();
        }

        private void UpdateTurboLineColor()
        {
            if (_questionsModel.TurboBoostIndicatorsCount <= 0)
            {
                _rightPanelView.SetTurboLineDefaultColor();
            }
            else
            {
                _rightPanelView.SetTurboLineTurboColor(_questionsModel.TurboBoostIndicatorsCount / 10f);
            }
        }

        private void SetupTurboBoostText()
        {
            _rightPanelView.TurboTextView.SetText(_localizationProvider.GetLocale(LocalizationKeys.TurboBoost));
        }

        private void UpdateTurboTextVisibility()
        {
            _rightPanelView.TurboTextView.SetTextVisibility(_questionsModel.TurboBoostIndicatorsCount > 0);
        }

        private async UniTaskVoid ProcessNextQuestion()
        {
            foreach (var answerView in _answersPanel.AnswerViews)
            {
                answerView.SetInteractable(false);
            }
            
            _turboTextView.SetTextAlpha(0);

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

        private void OnIsFinishingFlagChanged(bool isFinishing)
        {
            _rightPanelView.AnimateHide();
        }
    }
}