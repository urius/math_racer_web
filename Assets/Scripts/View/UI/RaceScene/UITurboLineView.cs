using UnityEngine;
using UnityEngine.UI;

namespace View.UI.RaceScene
{
    public class UITurboLineView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Color _turboActivatedColor2;
        
        private Color _defaultColor;

        private void Awake()
        {
            _defaultColor = _image.color;
            _turboActivatedColor2.a = _defaultColor.a;
        }

        public void ToDefaultColor()
        {
            _image.color = _defaultColor;
        }

        public void ToTurboColor(float turboPercent)
        {
            _image.color = Color.Lerp(_defaultColor, _turboActivatedColor2, turboPercent);
        }
        
        public void SetLineXScale(float xScale)
        {
            var scale = _rectTransform.localScale;
            scale.x = xScale;
            _rectTransform.localScale = scale;
        }
    }
}