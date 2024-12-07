using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.Extensions;
using View.UI.Common;
using View.UI.Popups.ContentPopup;

namespace View.UI.Popups.CarsPopup
{
    public class UICarsPopupItemView : MonoBehaviour, IUIContentPopupItem
    {
        public event Action<UICarsPopupItemView> ButtonClicked;
        
        [SerializeField] private Image _carIcon;
        [SerializeField] private Image _bgImage;
        [SerializeField] private RectTransform _firstParameterContainerTransform;
        [SerializeField] private TMP_Text _firstParameterText;
        [SerializeField] private RectTransform _firstParameterProgressTransform;
        [SerializeField] private RectTransform _secondParameterContainerTransform;
        [SerializeField] private TMP_Text _secondParameterText;
        [SerializeField] private RectTransform _secondParameterProgressTransform;
        [SerializeField] private UITextButtonView _button;
        [SerializeField] private TMP_Text _lockedText;
        [SerializeField] private Color _itemSelectedColor;

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

        public void SetSelectedState(bool isSelected)
        {
            _bgImage.color = isSelected ? _itemSelectedColor : Color.white;
            _button.SetInteractable(!isSelected);
        }

        public void SetLockedState(bool isLocked)
        {
            _lockedText.gameObject.SetActive(isLocked);
            _firstParameterContainerTransform.gameObject.SetActive(!isLocked);
            _secondParameterContainerTransform.gameObject.SetActive(!isLocked);
            _button.gameObject.SetActive(!isLocked);

            _carIcon.color = isLocked ? Color.black.SetAlpha(0.5f) : Color.white;
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

        public void SetButtonPriceText(string priceText)
        {
            _button.SetText(priceText);
        }

        public void SetLockedText(string text)
        {
            _lockedText.text = text;
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