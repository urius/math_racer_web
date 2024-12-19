using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.RaceScene
{
    public class UIRaceSceneRaceSchemaView : MonoBehaviour
    {
        [SerializeField] private Image _playerCarImage;
        [SerializeField] private RectTransform _playerCarRectTransform;
        [SerializeField] private Image _botCarImage;
        [SerializeField] private RectTransform _botCarRectTransform;
        [SerializeField] private TMP_Text _distanceText;

        public void SetDistanceText(string text)
        {
            _distanceText.text = text;
        }

        public void SetPlayerCarIconSprite(Sprite sprite)
        {
            _playerCarImage.sprite = sprite;
        }
        
        public void SetBotCarIconSprite(Sprite sprite)
        {
            _botCarImage.sprite = sprite;
        }
        
        public void SetPlayerCarPassedDistancePercent(float percent)
        {
            SetAnchorXMinMax(_playerCarRectTransform, percent); 
        }
        
        public void SetBotCarPassedDistancePercent(float percent)
        {
            SetAnchorXMinMax(_botCarRectTransform, percent); 
        }

        private void SetAnchorXMinMax(RectTransform rectTransform, float value)
        {
            var minAnchor = rectTransform.anchorMin;
            var maxAnchor = rectTransform.anchorMax;

            minAnchor.x = maxAnchor.x = value;

            rectTransform.anchorMin = minAnchor;
            rectTransform.anchorMax = maxAnchor;
        }
    }
}