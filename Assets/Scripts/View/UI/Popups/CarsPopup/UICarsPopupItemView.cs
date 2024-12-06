using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.UI.Common;
using View.UI.Popups.ContentPopup;

namespace View.UI.Popups.CarsPopup
{
    public class UICarsPopupItemView : MonoBehaviour, IUIContentPopupItem
    {
        public event Action<UICarsPopupItemView> ButtonClicked;
        
        [SerializeField] private Image _carIcon;
        [SerializeField] private TMP_Text _firstParameterText;
        [SerializeField] private RectTransform _firstParameterProgressTransform;
        [SerializeField] private TMP_Text _secondParameterText;
        [SerializeField] private RectTransform _secondParameterProgressTransform;
        [SerializeField] private UITextButtonView _button;

        private RectTransform _rectTransform;

        public RectTransform RectTransform => _rectTransform ? _rectTransform : transform as RectTransform;
        public Vector2 Size => RectTransform.sizeDelta;
        
        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            
            _button.ButtonClicked += OnButtonClicked;
        }

        private void OnDestroy()
        {
            _button.ButtonClicked -= OnButtonClicked;
        }

        public void SetCarIconSprite(Sprite sprite)
        {
            _carIcon.sprite = sprite;
        }

        public void SetParameterTexts(string firstParameterText, string secondParameterText)
        {
            _firstParameterText.text = firstParameterText;
            _secondParameterText.text = secondParameterText;
        }

        public void SetFirstParameterPercent(float percent)
        {
            SetXScale(_firstParameterProgressTransform, percent);
        }

        public void SetSecondParameterPercent(float percent)
        {
            SetXScale(_secondParameterProgressTransform, percent);
        }

        private void SetXScale(RectTransform rectTransform, float value)
        {
            var scale = rectTransform.localScale;
            scale.x = value;
            rectTransform.localScale = scale;
        }

        private void OnButtonClicked()
        {
            ButtonClicked?.Invoke(this);
        }
    }
}