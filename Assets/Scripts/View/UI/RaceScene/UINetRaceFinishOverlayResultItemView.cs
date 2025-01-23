using TMPro;
using UnityEngine;
using Utils;
using View.Helpers;

namespace View.UI.RaceScene
{
    public class UINetRaceFinishOverlayResultItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _placeText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _speedText;
        [SerializeField] private TMP_Text _answersText;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        public CanvasGroup CanvasGroup => _canvasGroup;

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetPlaceAndName(int place, string playerName, bool isLocalPLayer)
        {
            var placeStr = isLocalPLayer ? RichTextHelper.FormatYellow(place) : place.ToString();
            _placeText.SetText($"#{placeStr}");
            _nameText.SetText(isLocalPLayer ? RichTextHelper.FormatYellow(playerName) : playerName);
        }

        public void SetTimeText(int timeMs, bool isBestTime)
        {
            var timeStr = FormattingHelper.ToTimeFormatMinSec((int)(timeMs * 0.001));
            _timeText.SetText(isBestTime ? RichTextHelper.FormatGreen(timeStr) : timeStr);
        }

        public void SetSpeedText(string speedText, bool isBestSpeed)
        {
            _speedText.SetText(isBestSpeed ? RichTextHelper.FormatGreen(speedText) : speedText);
        }

        public void SetAnswers(int rightAnswers, int wrongAnswers, bool isBestRightAnswers)
        {
            var rightAnswersStr = isBestRightAnswers ? RichTextHelper.FormatGreen(rightAnswers) : rightAnswers.ToString();
            var wrongAnswersStr = wrongAnswers > 0 ? RichTextHelper.FormatRed(wrongAnswers) : wrongAnswers.ToString();

            _answersText.SetText($"{rightAnswersStr}/{wrongAnswersStr}");
        }
    }
}