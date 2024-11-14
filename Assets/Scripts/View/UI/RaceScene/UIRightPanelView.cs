using TMPro;
using UnityEngine;

namespace View.UI.RaceScene
{
    public class UIRightPanelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private UITurboLineView _turboLine;
        [SerializeField] private UITurboTextView _turboTextView;
        [SerializeField] private UIAnswersPanelView _answersPanel;

        public UIAnswersPanelView AnswersPanel => _answersPanel;
        public UITurboTextView TurboTextView => _turboTextView;

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
    }
}