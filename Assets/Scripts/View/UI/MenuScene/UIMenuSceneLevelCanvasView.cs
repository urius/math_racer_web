using TMPro;
using UnityEngine;

namespace View.UI.MenuScene
{
    public class UIMenuSceneLevelCanvasView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private RectTransform _levelProgressTransform;

        public void SetLevel(int level)
        {
            _levelText.text = level.ToString();
        }
        
        public void SetExpProgressLineXScale(float xScale)
        {
            var scale = _levelProgressTransform.localScale;
            scale.x = xScale;
            _levelProgressTransform.localScale = scale;
        }
    }
}