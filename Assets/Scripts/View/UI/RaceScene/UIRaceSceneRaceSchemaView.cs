using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.RaceScene
{
    public class UIRaceSceneRaceSchemaView : MonoBehaviour
    {
        [SerializeField] private Image _playerCarImage;
        [SerializeField] private RectTransform _playerCarRectTransform;
        [SerializeField] private Image[] _opponentCarImages;
        [SerializeField] private TMP_Text _distanceText;
        
        private RectTransform[] _opponentCarRectTransforms;

        private void Awake()
        {
            _opponentCarRectTransforms = _opponentCarImages.Select(i => i.transform as RectTransform).ToArray();
            foreach (var rectTransform in _opponentCarRectTransforms)
            {
                rectTransform.gameObject.SetActive(false);
            }
        }

        public void SetDistanceText(string text)
        {
            _distanceText.text = text;
        }

        public void SetPlayerCarIconSprite(Sprite sprite)
        {
            _playerCarImage.sprite = sprite;
        }
        
        public void SetOpponentCarIconSprite(int opponentIndex, Sprite sprite)
        {
            _opponentCarImages[opponentIndex].gameObject.SetActive(true);
            _opponentCarImages[opponentIndex].sprite = sprite;
        }
        
        public void SetPlayerCarPassedDistancePercent(float percent)
        {
            SetAnchorXMinMax(_playerCarRectTransform, percent); 
        }
        
        public void SetOpponentCarPassedDistancePercent(int opponentIndex, float percent)
        {
            SetAnchorXMinMax(_opponentCarRectTransforms[opponentIndex], percent); 
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