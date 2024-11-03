using TMPro;
using UnityEngine;

namespace View.UI.RaceScene
{
    public class UIRightPanelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private RectTransform _timerLineRectTransform;
        [SerializeField] private UIAnswersPanelView _answersPanel;

        public UIAnswersPanelView AnswersPanel => _answersPanel;

        public void SetQuestionText(string text)
        {
            _questionText.text = text;
        }
    }
}