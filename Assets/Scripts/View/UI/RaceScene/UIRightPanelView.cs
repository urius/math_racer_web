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

        private void SetHiddenPosition()
        {
            _rectTransform.SetAnchoredXPosition(_rectTransform.sizeDelta.x);
        }
    }
}