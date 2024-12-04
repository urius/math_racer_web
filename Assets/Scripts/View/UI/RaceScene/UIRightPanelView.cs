using Cysharp.Threading.Tasks;
using Extensions;
using TMPro;
using UnityEngine;

namespace View.UI.RaceScene
{
    public class UIRightPanelView : MonoBehaviour
    {
        private const float ShowHideAnimationDuration = 0.2f;
        
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private UITurboLineView _turboLine;
        [SerializeField] private UITurboTextView _turboTextView;
        [SerializeField] private UIAnswersPanelView _answersPanel;
        [SerializeField] private UIBoostIndicatorView[] _boostIndicators;
        
        private RectTransform _rectTransform;

        public UIAnswersPanelView AnswersPanel => _answersPanel;
        public UITurboTextView TurboTextView => _turboTextView;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            
            SetHiddenPosition();
        }

        public void SetQuestionText(string text)
        {
            _questionText.text = text;
        }

        public void SetTurboTimerLineXScale(float xScale)
        {
            _turboLine.SetLineXScale(xScale);
        }
        
        public void SetTurboLineDefaultColor()
        {
            _turboLine.ToDefaultColor();
        }
        
        public void SetTurboLineTurboColor(float turboPercent)
        {
            _turboLine.ToTurboColor(turboPercent);
        }
        
        public void AnimateShow()
        {
            SetHiddenPosition();
            _rectTransform.LeanMoveX(0, ShowHideAnimationDuration)
                .setDelay(0.5f)
                .setEaseOutQuad()
                .setIgnoreTimeScale(true);
        }

        public void AnimateHide()
        {
            var sizeX = _rectTransform.sizeDelta.x;
            _rectTransform.LeanMoveX(sizeX, ShowHideAnimationDuration)
                .setIgnoreTimeScale(true)
                .setEaseInQuad()
                .setOnComplete(() => gameObject.SetActive(false));
        }

        public void ShowGreenIndicator(int index)
        {
            _boostIndicators[index].ToGreenColor();
            _boostIndicators[index].FadeIn().Forget();
        }
        
        public void HideRedIndicator(int index)
        {
            _boostIndicators[index].ToRedColor();
            _boostIndicators[index].FadeOut().Forget();
        }

        public void SetEnabledIndicators(int turboBoostIndicatorsCount)
        {
            for (var i = 0; i < turboBoostIndicatorsCount; i++)
            {
                _boostIndicators[i].ToGreenColor();
                _boostIndicators[i].SetAlpha(i < turboBoostIndicatorsCount ? 1 : 0);
            }
        }

        public UniTask AnimateTurboBoost()
        {
            var result = UniTask.CompletedTask;

            for (var i = 0; i < _boostIndicators.Length; i++)
            {
                var indicatorView = _boostIndicators[_boostIndicators.Length - 1 - i];
                indicatorView.ToYellowColor();

                result = indicatorView.FadeOut(0.5f + 0.1f * i);
            }

            return result;
        }

        private void SetHiddenPosition()
        {
            _rectTransform.SetAnchoredXPosition(_rectTransform.sizeDelta.x);
        }
    }
}