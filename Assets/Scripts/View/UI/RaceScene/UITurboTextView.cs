using TMPro;
using UnityEngine;
using View.Extensions;

namespace View.UI.RaceScene
{
    public class UITurboTextView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Color[] _colors;

        public float TextAlpha => _text.color.a;
        private void Awake()
        {
            for (var i = 0; i < _colors.Length; i++)
            {
                _colors[i].a = _text.color.a;
            }
        }

        public void SetTextAlpha(float alpha)
        {
            _text.SetAlpha(alpha);
        }

        public void SetTextVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetTextColorIndex(int colorIndex)
        {
            _text.color = colorIndex <= 0 ? _colors[0] :
                colorIndex < _colors.Length ? _colors[colorIndex] : _colors[^1];
        }
    }
}